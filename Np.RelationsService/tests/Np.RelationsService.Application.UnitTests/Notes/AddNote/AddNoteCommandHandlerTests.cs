using FluentAssertions;
using Moq;
using Np.RelationsService.Application.Notes.AddNote;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.RootEntries;
using Xunit;

namespace Np.RelationsService.Application.UnitTests.Notes.AddNote
{
    public class AddNoteCommandHandlerTests
    {
        private readonly AddNoteCommand _command = new(NoteData.Note_1.Id, Guid.Empty);

        private readonly AddNoteCommandHandler _sut;

        private readonly Mock<INotesRepository> _notesRepositoryMock;

        private readonly Mock<IRootEntriesRepository> _rootEntriesRepositoryMock;

        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddNoteCommandHandlerTests()
        {
            _notesRepositoryMock = new Mock<INotesRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _rootEntriesRepositoryMock = new Mock<IRootEntriesRepository>();
            _sut = new(
                _notesRepositoryMock.Object,
                _rootEntriesRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldCheckNoteExisting()
        {
            await _sut.Handle(_command ,default);

            _notesRepositoryMock.Verify(
                x => x.Contains(It.Is<Guid>(id => id.Equals(_command.NoteId))),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ReturnFailedResult_IfNoteAlreadyExists()
        {
            _notesRepositoryMock
                .Setup(x => x.Contains(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));

            var result = await  _sut.Handle(_command, default);

            result.IsFailed.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldAddNoteToRepository_IfNoteNonExists()
        {
            await _sut.Handle(_command, default);

            _notesRepositoryMock.Verify(x => x.Add(It.IsAny<Note>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_IfNoteAdded()
        {
            var result = await _sut.Handle(_command, default);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldSaveChanges_IfNoteAdded()
        {
            var result = await _sut.Handle(_command, default);

            _unitOfWorkMock.Verify(
                x=> x.SaveChangesAsync(It.IsAny<CancellationToken>()), 
                Times.Once);
        }


        [Fact]
        public async Task Handle_ShouldAddRootEntry_IfNoteAdded()
        {
            var result = await _sut.Handle(_command, default);

            _rootEntriesRepositoryMock.Verify(
                x => x.Add(It.IsAny<RootEntry>()),
                Times.Once);
        }
    }
}
