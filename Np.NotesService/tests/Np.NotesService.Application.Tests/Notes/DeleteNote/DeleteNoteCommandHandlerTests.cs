using FluentAssertions;
using Moq;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Application.Notes.RemoveNote;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;
using Np.NotesService.Domain.Notes.Events;
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
        private readonly RemoveNoteCommand _command = new RemoveNoteCommand(Guid.NewGuid());

        private readonly RemoveNoteCommandHandler _handler;

        private readonly Mock<INotesRepository> _notesRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public DeleteNoteCommandHandlerTests()
        {
            _notesRepositoryMock = new Mock<INotesRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _handler = new RemoveNoteCommandHandler(
                _notesRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        private void SetupMockToReturnExistingNote()
        {
            _notesRepositoryMock
                .Setup(x => x.GetNoteById(It.Is<Guid>(x => x.Equals(_command.NoteId))))
                .Returns(()=> {
                    var note = Note.Create("Mock Note", new Mock<IDateTimeProvider>().Object);
                    note.ClearDomainEvents();
                    return Task.FromResult<Note?>(note);
                });
        }


        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenNoteNotFound()
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
        public async Task Handle_ShouldCallRepositoryToDeleteNote_WhenNoteExists()
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
        public async Task Handle_ShouldSaveChanges_WhenDeleteExistingNote()
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
        public async Task Handle_ShouldReturnSuccess_WhenDeleteExistingNote()
        {
            // Arrange
            SetupMockToReturnExistingNote();

            // Act
            var result = await _handler.Handle(_command, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldThrowConcurrencyException_WhenSaveChangesThrowsInvalidOperationException()
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

        [Fact]
        public async Task Handle_ShouldAddEvent_IfNoteRemoved()
        {
            Note? actualNote = null;
            SetupMockToReturnExistingNote();
            _notesRepositoryMock
                .Setup(x => x.Delete(It.IsAny<Note>()))
                .Callback<Note>(n=> actualNote = n);

            await _handler.Handle(_command, default);

            actualNote!.DomainEvents.Should().HaveCount(1);
            actualNote!.DomainEvents.First().Should().Be(new NoteRemovedEvent(actualNote.Id));
        }
    }
}
