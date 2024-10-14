
using FluentAssertions;
using Np.RelationsService.Domain.Notes;
using Np.RelationsService.Domain.Relations;
using Xunit;

namespace Np.RelationsService.Domain.UnitTests.Notes
{
    public class NoteTests
    {
        private readonly Guid _noteId = new("6bcf248d-312b-4a75-b54a-0a477885b76d");

        [Fact]
        public void Create_ShouldCreateNote()
        {
            // Act
            var note = Note.Create(_noteId);

            // Assert
            note.IsSuccess.Should().BeTrue();
            note.Value.Id.Should().Be(_noteId);
        }
    }
}
