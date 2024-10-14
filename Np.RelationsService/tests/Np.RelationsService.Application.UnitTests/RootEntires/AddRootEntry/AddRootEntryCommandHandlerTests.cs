
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Np.RelationsService.Application.Exceptions;
using Np.RelationsService.Application.RootEntries.AddRootEntry;
using Np.RelationsService.Application.UnitTests.Notes;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.RootEntries;
using Xunit;

namespace Np.RelationsService.Application.UnitTests.RootEntires.AddRootEntry
{
    public class AddRootEntryCommandHandlerTests
    {
        private readonly AddRootEntryCommand _command = new(NoteData.Note_1.Id);

        private readonly Mock<INotesRepository> _notesRepostoryMock;
        private readonly Mock<IRootEntriesRepository> _rootEntriesRepostoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        private readonly AddRootEntryCommandHandler _sut;

        public AddRootEntryCommandHandlerTests()
        {
            _notesRepostoryMock = new Mock<INotesRepository>();
            _rootEntriesRepostoryMock = new Mock<IRootEntriesRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _sut = new AddRootEntryCommandHandler(
                _notesRepostoryMock.Object,
                _rootEntriesRepostoryMock.Object,
                _unitOfWorkMock.Object);
        }

        private void SetupNoteInRepository()
        {
            _notesRepostoryMock
                .Setup(x=> x.GetNoteById(It.Is<Guid>(id=> id.Equals(_command.NoteId))))
                .Returns(Task.FromResult<Note?>(NoteData.Note_1));
        }

        [Fact]
        public async Task Handler_ShouldGetNoteFromRepository()
        {
            await _sut.Handle(_command, default);

            _notesRepostoryMock.Verify(
                x=> x.GetNoteById(
                    It.Is<Guid>(id=> id.Equals(_command.NoteId))),
                Times.Once());
        }

        [Fact]
        public async Task Handler_ShouldFailedResult_IfNoteNotFound()
        {
            _notesRepostoryMock
                .Setup(x => x.GetNoteById(It.IsAny<Guid>()))
                .Returns(Task.FromResult<Note?>(null));

            var result = await _sut.Handle(_command, default);

            result.IsFailed.Should().BeTrue();
            result.Error.Should().Be(Error.Null);
        }

        [Fact]
        public async Task Handler_ShouldFailedResult_IfNoteAlreadyRootEntry()
        {
            SetupNoteInRepository();
            _rootEntriesRepostoryMock.Setup(
                x => x.IsEntryRoot(
                    It.Is<Note>(note => note.Id.Equals(_command.NoteId)),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            var result = await _sut.Handle(_command, default);

            result.IsFailed.Should().BeTrue();
            result.Error.Should().Be(RootEntryErrors.Duplication);
        }

        [Fact]
        public async Task Handle_ShouldAddRootEntryInRepository_IfNoteNonEntryRoot()
        {
            SetupNoteInRepository(); 

            await _sut.Handle(_command, default);

            _rootEntriesRepostoryMock.Verify(x=> x.Add(It.IsAny<RootEntry>()), Times.Once());
        }

        [Fact]
        public async Task Handle_ShouldSaveChanges_IfNoteAdded()
        {
            SetupNoteInRepository();

            await _sut.Handle(_command, default);

            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }


        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_IfNoteAdded()
        {
            SetupNoteInRepository();

            var result = await _sut.Handle(_command, default);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldThrowConcurrnecyException_IfSaveChangesThrowsDbUpdateConcurrencyException()
        {
            SetupNoteInRepository();
            _unitOfWorkMock
                .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Throws<DbUpdateConcurrencyException>();

            await Assert.ThrowsAsync<ConcurrencyException>(()=>  _sut.Handle(_command, default));
        }
    }
}
