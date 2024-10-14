
using FluentAssertions;
using Moq;
using Np.RelationsService.Application.Notes.RemoveNote;
using Np.RelationsService.Application.Relations.RemoveRelation;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.Relations.Events;
using Np.RelationsService.Domain.RootEntries;
using Xunit;

namespace Np.RelationsService.Application.UnitTests.Notes.RemoveNote;

public class RemoveNoteCommandHandlerTest
{
    private readonly Note _testingNote = NoteData.Note_1;
    private readonly RemoveNoteCommand _command = new(NoteData.Note_1.Id);

    private readonly Mock<INotesRepository> _noteRepositoryMock;
    private readonly Mock<IRelationsRepository> _relationsRepositoryMock;
    private readonly Mock<IRootEntriesRepository> _rootEntriesRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly RemoveNoteCommandHandler _sut;

    public RemoveNoteCommandHandlerTest()
    {
        _noteRepositoryMock = new Mock<INotesRepository>();
        _relationsRepositoryMock = new Mock<IRelationsRepository>();
        _rootEntriesRepositoryMock = new Mock<IRootEntriesRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _sut = new RemoveNoteCommandHandler(
            _noteRepositoryMock.Object,
            _relationsRepositoryMock.Object,
            _rootEntriesRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    private void SetupNotesReposirotyReturnNote()
    {
        _noteRepositoryMock
            .Setup(x=> x.GetNoteById(It.Is<Guid>(id=> id.Equals(_testingNote.Id))))
            .Returns(Task.FromResult<Note?>(_testingNote));
    }

    [Fact]
    public async Task Handle_ShouldRemoveRootEntry_IfNoteHaveRootEntry()
    {
        SetupNotesReposirotyReturnNote();
        var rootEntry = RootEntry.Create(_testingNote).Value;
        _rootEntriesRepositoryMock
            .Setup(x => x.GetRootEntryByNoteId(
                It.Is<Guid>(id => id.Equals(_testingNote.Id))))
            .Returns(Task.FromResult<RootEntry?>(rootEntry));

        await _sut.Handle(_command, default);

        _rootEntriesRepositoryMock.Verify(
            x => x.Remove(
                It.Is<RootEntry>(r => r.Equals(rootEntry))),
                Times.Once);
    }


    [Fact]
    public async Task Handle_ShouldRemoveRelations_IfNoteHaveRealtions()
    {
        SetupNotesReposirotyReturnNote();
        List<Relation> incomingRelations = [
            Relation.Create(NoteData.Note_1_1, _testingNote).Value,
            Relation.Create(NoteData.Note_1_2, _testingNote).Value];
        List<Relation> outgoingRelations = [
            Relation.Create(_testingNote, NoteData.Note_0).Value];
        _relationsRepositoryMock
            .Setup(x => x.GetIncomingRelations(It.Is<Guid>(id => id.Equals(_testingNote.Id))))
            .Returns(Task.FromResult<IEnumerable<Relation>>(incomingRelations));

        _relationsRepositoryMock
            .Setup(x => x.GetOutgoingRelations(It.Is<Guid>(id => id.Equals(_testingNote.Id))))
            .Returns(Task.FromResult<IEnumerable<Relation>>(outgoingRelations));

        await _sut.Handle(_command, default);

        foreach(var relation in outgoingRelations.Union(incomingRelations))
        {
            relation.DomainEvents.Should().HaveCount(1);
            relation.DomainEvents.First()
                .Should().Be(new RelationRemovedEvent(
                    relation.Id, 
                    relation.Outgoing.Id, 
                    relation.Incoming.Id));

            _relationsRepositoryMock.Verify(
                x => x.RemoveRelation(It.Is<Relation>(r => r.Equals(relation))), 
                Times.Once);
        }
    }
}
