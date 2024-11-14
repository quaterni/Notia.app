using FluentAssertions;
using Moq;
using Np.NotesService.Application.Exceptions;
using Np.NotesService.Application.Notes.UpdateNote;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;
using Np.NotesService.Domain.Notes.Events;
using Xunit;

namespace Np.NotesService.Application.Tests.Notes.UpdateNote;

public class UpdateNoteCommandHandlerTests
{
    private readonly Note _updatingNote;
    private readonly UpdateNoteCommand _command;
    private readonly UpdateNoteCommandHandler _handler;

    private readonly Mock<INotesRepository> _notesRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public UpdateNoteCommandHandlerTests()
    {
        _updatingNote = Note.Create("Note to update", Guid.Empty, new Mock<IDateTimeProvider>().Object);
        _updatingNote.ClearDomainEvents();
        _command = new UpdateNoteCommand("Updated Data", _updatingNote.Id, "") { UserId = Guid.Empty};

        _notesRepositoryMock = new();
        _unitOfWorkMock = new();

        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        _handler = new UpdateNoteCommandHandler(
            _notesRepositoryMock.Object, 
            _unitOfWorkMock.Object,
            dateTimeProviderMock.Object);
    }

    private void SetupMockToGetNote()
    {
        _notesRepositoryMock
            .Setup(x => x.GetNoteById(It.Is<Guid>(id => id.Equals(_command.NoteId))))
            .Returns(Task.FromResult<Note?>(_updatingNote));
    }

    [Fact]
    public async Task Handle_Should_CallReposotiryToFindNote()
    {
        // Act
        await _handler.Handle(_command, default);

        // Assert
        _notesRepositoryMock.Verify(
            x => x.GetNoteById(It.Is<Guid>(id => id.Equals(_command.NoteId))));
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenNoteNotFound()
    {
        // Arrange
        _notesRepositoryMock
            .Setup(x => x.GetNoteById(It.Is<Guid>(id => id.Equals(_command.NoteId))))
            .Returns(Task.FromResult<Note?>(null));

        // Act
        var result = await _handler.Handle(_command, default);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Message.Should().Be(UpdateNoteErrors.NotFound);
    }

    [Fact]
    public async Task Handle_Should_CallRepositoryToUpdateNote_WhenNoteExisting()
    {
        // Arrange
        SetupMockToGetNote();

        // Act
        var result = await _handler.Handle(_command, default);

        // Assert
        _notesRepositoryMock.Verify(
            x => x.Update(It.Is<Note>(note => note.Id.Equals(_updatingNote.Id))),
            Times.Once);
    }


    [Fact]
    public async Task Handle_Should_SaveChanges_WhenNoteExisting()
    {
        // Arrange
        SetupMockToGetNote();

        // Act
        var result = await _handler.Handle(_command, default);

        // Assert
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.Is<CancellationToken>(x => x.Equals(default))),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_WhenNoteUpdated()
    {
        // Arrange
        SetupMockToGetNote();

        // Act
        var result = await _handler.Handle(_command, default);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ThrowConcurrencyException_WhenSaveChangesThrowsInvalidOperationException()
    {
        // Arrange
        SetupMockToGetNote();
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.Is<CancellationToken>(x => x.Equals(default))))
            .Throws<InvalidOperationException>();
            

        // Act
        var exception = await Assert.ThrowsAsync<ConcurrencyException>(
            ()=> _handler.Handle(_command, default));

        // Assert
        exception.InnerException.Should().NotBeNull();
        exception.InnerException.Should().BeOfType<InvalidOperationException>();
    }

    [Fact]
    public async Task Handle_ShouldAddEvent_IfNoteChanged()
    {
        SetupMockToGetNote();
        Note? actualNote = null;
        _notesRepositoryMock
            .Setup(x=> x.Update(It.IsAny<Note>()))
            .Callback<Note>(x => actualNote = x);

        await _handler.Handle(_command, default);

        actualNote!.DomainEvents.Should().HaveCount(1);
        actualNote.DomainEvents.First().Should().Be(new NoteChangedEvent(actualNote.Id));
    }
}
