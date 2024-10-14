using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Np.RelationsService.Application.Abstractions.Messaging;
using Np.RelationsService.Application.Exceptions;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.RootEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Np.RelationsService.Application.UnitTests.RootEntires.RemoveRootEntry
{
    public class RemoveRootEntryCommandHandlerTests
    {
        private readonly RemoveRootEntryCommand _command = new(RootEntryData.DefaultRootEntry.Id);

        private readonly Mock<IRootEntriesRepository> _rootEntriesRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        private readonly RemoveRootEntryCommandHandler _sut;

        public RemoveRootEntryCommandHandlerTests()
        {
            _rootEntriesRepositoryMock = new Mock<IRootEntriesRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _sut = new RemoveRootEntryCommandHandler(
                _rootEntriesRepositoryMock.Object, 
                _unitOfWorkMock.Object);

            _rootEntriesRepositoryMock
                .Setup(x=> x.GetRootEntryById(It.Is<Guid>(id => id.Equals(_command.RootEntryId))))
                .Returns(Task.FromResult<RootEntry?>(RootEntryData.DefaultRootEntry));
        }

        [Fact]
        public async Task Handle_ShouldFailedResult_IfRootEntryNotFound()
        {
            _rootEntriesRepositoryMock
                .Setup(x => x.GetRootEntryById(
                    It.Is<Guid>(id => id.Equals(_command.RootEntryId))))
                .Returns(Task.FromResult<RootEntry?>(null));

            var result = await _sut.Handle(_command, default);

            result.IsFailed.Should().BeTrue();
            result.Error.Should().Be(RootEntryErrors.NotFound);
        }

        [Fact]
        public async Task Handle_ShouldRemoveRootEntry_IfRootEntryFound()
        {
            var rootEntry = RootEntryData.DefaultRootEntry;

            var result = await _sut.Handle(_command, default);

            _rootEntriesRepositoryMock.Verify(
                x=> x.Remove(It.Is<RootEntry>(r=> r.Equals(rootEntry))),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldSaveChanges_IfRootEntryRemoved()
        {
            var result = await _sut.Handle(_command, default);

            _unitOfWorkMock.Verify(
                x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_ThrowsConcurrencyException_IfSaveChangesThrowsDbUpdateConcurrencyException()
        {
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Throws<DbUpdateConcurrencyException>();

            await Assert.ThrowsAsync<ConcurrencyException>(()=>  _sut.Handle(_command, default));         
        }

    }
}
