
using FluentAssertions;
using Moq;
using Np.RelationsService.Application.Relations.AddRelation;
using Np.RelationsService.Application.UnitTests.Notes;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.Relations.Events;
using Xunit;

namespace Np.RelationsService.Application.UnitTests.Relations.AddRelation;

public class AddRelationCommandHandlerTests
{
    private readonly AddRelationCommand _command = new(new Guid("11111111-122b-4a75-b54a-0a477885b76d"), new Guid("21111111-122b-4a75-b54a-0a477885b76d"), "") {
     UserId = Guid.Empty
    };

    private readonly Mock<INotesRepository> _notesRepositoryMock;
    private readonly Mock<IRelationsRepository> _relationsRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly AddRelationCommandHandler _handler;

    public AddRelationCommandHandlerTests()
    {
        _notesRepositoryMock = new Mock<INotesRepository>();
        _relationsRepositoryMock = new Mock<IRelationsRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _handler = new AddRelationCommandHandler(
            _notesRepositoryMock.Object,
            _relationsRepositoryMock.Object,
            _unitOfWorkMock.Object);

    }

    private void SetupNotesRepository()
    {
        var outgoingNote = Note.Create(_command.OutgoingNoteId, Guid.Empty).Value;
        var incomingNote = Note.Create(_command.IncomingNoteId, Guid.Empty).Value;

        _notesRepositoryMock
            .Setup(x => x.GetNoteById(
                It.Is<Guid>(id => id.Equals(outgoingNote.Id))))
            .Returns(Task.FromResult<Note?>(outgoingNote));
        _notesRepositoryMock
            .Setup(x => x.GetNoteById(
                It.Is<Guid>(id => id.Equals(incomingNote.Id))))
            .Returns(Task.FromResult<Note?>(incomingNote));
    }

    [Fact]
    public async Task Handle_AddEvent_IfRelationAdded()
    {

        SetupNotesRepository();
        Relation? actualRelation = null;
        _relationsRepositoryMock
            .Setup(x => x.AddRelation(It.IsAny<Relation>()))
            .Callback<Relation>(x => actualRelation = x);

        await _handler.Handle(_command, default);

        actualRelation.Should().NotBeNull();
        actualRelation!.DomainEvents.Should().HaveCount(1);
        actualRelation!.DomainEvents.First()
            .Should()
            .Be(new RelationCreatedEvent(
                actualRelation.Id, 
                actualRelation.Outgoing.Id, 
                actualRelation.Incoming.Id));

    }
}
