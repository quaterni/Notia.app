using FluentAssertions;
using Moq;
using Np.NotesService.Application.Notes.AddNote;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;
using Xunit;

namespace Np.NotesService.Application.Tests.Notes.AddNote
{
    public class AddNoteCommandHandlerTests
    {
        private readonly AddNoteCommand _command = new AddNoteCommand("Title\n\nContent");

        private readonly Mock<INotesRepository> _notesRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        private readonly AddNoteCommandHandler _handler;

        public AddNoteCommandHandlerTests()
        {
            _notesRepositoryMock = new Mock<INotesRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            var dateTimeProviderMock = new Mock<IDateTimeProvider>();

            _handler = new AddNoteCommandHandler(
                _notesRepositoryMock.Object, 
                _unitOfWorkMock.Object,
                dateTimeProviderMock.Object); 
        }

        [Fact]
        public async Task Handle_Should_CallRepository_WhenAddNote()
        {
            // Act
            var result = await _handler.Handle(_command, default);

            // Assert
            _notesRepositoryMock.Verify(x=> x.Add(It.IsAny<Note>()), Times.Once);
        }


        [Fact]
        public async Task Handle_Should_CallSaveChanges_WhenAddNote()
        {
            // Act
            var result = await _handler.Handle(_command, default);

            // Assert
            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.Is<CancellationToken>(x=> x.Equals(default))),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnId_WhenAddNote()
        {
            // Arrange
            Guid? idAddedInRepository = null;
            _notesRepositoryMock
                .Setup(x => x.Add(It.IsAny<Note>()))
                .Callback<Note>(note => idAddedInRepository = note.Id);

            // Act
            var result = await _handler.Handle(_command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
            idAddedInRepository.Should().NotBeNull();
            result.Value.Should().Be((Guid)idAddedInRepository!);
        }
    }
}
