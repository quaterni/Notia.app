
using FluentAssertions;
using Moq;
using Np.RelationsService.Application.Relations.RemoveRelation;
using Np.RelationsService.Domain.Abstractions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Np.RelationsService.Domain.Relations.Events;
using Xunit;

namespace Np.RelationsService.Application.UnitTests.Relations.RemoveRelation;

public class RemoveRelationCommandHandlerTests
{
    private readonly RemoveRelationCommand _command = new(new Guid("11111111-122b-4a75-b54a-0a477885b76d"));

    private readonly Mock<IRelationsRepository> _relationsRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    private readonly RemoveRelationCommandHandler _sut;

    public RemoveRelationCommandHandlerTests()
    {
        _relationsRepositoryMock = new();
        _unitOfWorkMock = new();

        _sut = new RemoveRelationCommandHandler(
            _relationsRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    private void SetupRelationsRepostiory()
    {
        var relation = Relation.Create(
            Note.Create(new Guid("21111111-122b-4a75-b54a-0a477885b76d")).Value,
            Note.Create(new Guid("31111111-122b-4a75-b54a-0a477885b76d")).Value).Value;

        _relationsRepositoryMock
            .Setup(x => x.GetRelationById(It.Is<Guid>(id => id.Equals(_command.RelationId))))
            .Returns(Task.FromResult<Relation?>(relation));
    }

    [Fact]
    public async Task Handle_AddEvent_IfRelationRemoved()
    {
        SetupRelationsRepostiory();
        Relation? relation = null;
        _relationsRepositoryMock
            .Setup(x=> x.RemoveRelation(It.IsAny<Relation>()))
            .Callback<Relation>(x => relation = x);

        await _sut.Handle(_command, default);

        relation.Should().NotBeNull();
        relation!.DomainEvents.Should().HaveCount(1);
        relation.DomainEvents.First()
            .Should()
            .Be(new RelationRemovedEvent(
                relation.Id, 
                relation.Outgoing.Id, 
                relation.Incoming.Id));
    }

}
