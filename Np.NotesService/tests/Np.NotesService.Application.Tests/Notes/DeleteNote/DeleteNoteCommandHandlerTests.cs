using FluentAssertions;
using Moq;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Application.Notes.DeleteNote;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Np.NotesService.Application.Tests.Notes.DeleteNote
{
    public class DeleteNoteCommandHandlerTests
    {
        private readonly DeleteNoteCommand _command = new DeleteNoteCommand(Guid.NewGuid());

        private readonly DeleteNoteCommandHandler _handler;

        private readonly Mock<INotesRepository> _notesRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public DeleteNoteCommandHandlerTests()
        {
            _notesRepositoryMock = new Mock<INotesRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new DeleteNoteCommandHandler(
                _notesRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        private void SetupMockToReturnExistingNote()
        {
            _notesRepositoryMock
                .Setup(x => x.GetNoteById(It.Is<Guid>(x => x.Equals(_command.NoteId))))
                .Returns(Task.FromResult(Note.Create("Mock Note", new Mock<IDateTimeProvider>().Object))!);
        }


        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenNoteNotFound()
        {
            // Arrange
            _notesRepositoryMock
                .Setup(x => x.GetNoteById(It.Is<Guid>(x => x.Equals(_command.NoteId))))
                .Returns(Task.FromResult<Note?>(null));

            // Act
            var result = await _handler.Handle(_command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_CallRepositoryToGetNote()
        {
            // Act
            await _handler.Handle(_command, default);

            // Assert
            _notesRepositoryMock.Verify(
                x => x.GetNoteById(It.Is<Guid>(x=> x.Equals(_command.NoteId))), 
                Times.Once);
        }


        [Fact]
        public async Task Handle_Should_CallRepositoryToDeleteNote_WhenNoteExists()
        {
            // Arrange
            SetupMockToReturnExistingNote();

            // Act
            await _handler.Handle(_command, default);

            // Assert
            _notesRepositoryMock.Verify(
                x => x.Delete(It.IsAny<Note>()),
                Times.Once);
        }


        [Fact]
        public async Task Handle_Should_SaveChanges_WhenDeleteExistingNote()
        {
            // Arrange
            SetupMockToReturnExistingNote();

            // Act
            await _handler.Handle(_command, default);

            // Assert
            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.Is<CancellationToken>(x=> x.Equals(default))),
                Times.Once);
        }

        [Fact]
        public async Task Handle_Should_ReturnSuccess_WhenDeleteExistingNote()
        {
            // Arrange
            SetupMockToReturnExistingNote();

            // Act
            var result = await _handler.Handle(_command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_ThrowConcurrencyException_WhenSaveChangesThrowsInvalidOperationException()
        {
            // Arrange
            SetupMockToReturnExistingNote();
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();

            // Act
            var exception = await Assert.ThrowsAsync<ConcurrencyException>(()=> _handler.Handle(_command, default));

            // Assert
            exception.InnerException.Should().NotBeNull();
            exception.InnerException.Should().BeOfType<InvalidOperationException>();
        }
    }
}
