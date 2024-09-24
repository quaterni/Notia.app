using Moq;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;
using Np.NotesService.Domain.Notes.Events;
using Xunit;

namespace Np.NotesService.Domain.Tests.Notes
{
    public class NoteTests
    {
        private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;

        public NoteTests()
        {
            _dateTimeProviderMock = new Mock<IDateTimeProvider>();

            _dateTimeProviderMock.Setup(p => p.GetCurrentTime()).Returns(new DateTime(123456789));
        }


        [Fact]
        public void Create_Should_SetCreatedTimeFromTimeProvider()
        {
            // Arrange
            var dateTimeProvider = _dateTimeProviderMock.Object;
            var expectedTime = dateTimeProvider.GetCurrentTime();

            // Act
            var note = Note.Create("test", dateTimeProvider);

            // Assert
            Assert.Equal<DateTime>(expectedTime, note.CreateTime);
        }


        [Fact]
        public void Create_Should_SetLastUpdateTimeFromTimeProvider()
        {
            // Arrange
            var dateTimeProvider = _dateTimeProviderMock.Object;
            var expectedTime = dateTimeProvider.GetCurrentTime();

            // Act
            var note = Note.Create("test", dateTimeProvider);

            // Assert
            Assert.Equal<DateTime>(expectedTime, note.LastUpdateTime);
        }

        [Theory]
        [ClassData(typeof(NoteTestData))]
        public void Create_Should_SetTitleFromData(NoteTestDto noteTestDto)
        {
            // Act
            var note = Note.Create(noteTestDto.Data, _dateTimeProviderMock.Object);

            // Assert
            Assert.Equal(noteTestDto.ExpectedTitle, note.Title);
            Assert.Equal(noteTestDto.ExpectedContent, note.Content);
        }

        [Fact]
        public void Create_Should_AddNoteCreatedDomainEvent()
        {
            // Act
            var note = Note.Create(NoteTestData.OneLineNote.Data, _dateTimeProviderMock.Object);
            var createdDomainEvent = note.DomainEvents
                .FirstOrDefault(e => e.GetType().Equals(typeof(NoteCreatedDomainEvent))) 
                    as NoteCreatedDomainEvent;

            // Assert
            Assert.NotNull(createdDomainEvent);
            Assert.Equal(note.Id, createdDomainEvent.NoteId);
        }

        [Theory]
        [ClassData(typeof(NoteTestData))]
        public void UpdateNote_Should_SetTitleFromData(NoteTestDto noteTestDto)
        {
            // Arrange
            var note = Note.Create("My note", _dateTimeProviderMock.Object);

            // Act
            note.UpdateNote(noteTestDto.Data, _dateTimeProviderMock.Object);

            // Assert
            Assert.Equal(noteTestDto.ExpectedTitle, note.Title);
            Assert.Equal(noteTestDto.ExpectedContent, note.Content);
        }

        [Fact]
        public void UpdateNote_Should_AddNoteUpdatedDomainEvent()
        {
            // Arrange
            var note = Note.Create("My note", _dateTimeProviderMock.Object);

            // Act
            note.UpdateNote("New Title", _dateTimeProviderMock.Object);
            var updatedDomainEvent = note.DomainEvents.FirstOrDefault(e=> e is NoteUpdatedDomainEvent)
                as NoteUpdatedDomainEvent;

            // Assert
            Assert.NotNull(updatedDomainEvent);
            Assert.Equal(note.Id, updatedDomainEvent.NoteId);
        }

        [Fact]
        public void UpdateNote_Should_SetLastUpdatedTimeFromDateTimeProvider()
        {
            // Arrange
            var note = Note.Create("My note", _dateTimeProviderMock.Object);
            var expectedLastUpdated = new DateTime(2024, 9, 19, 0, 9, 35);
            _dateTimeProviderMock.Setup(x=> x.GetCurrentTime()).Returns(expectedLastUpdated);

            // Act
            note.UpdateNote("New title", _dateTimeProviderMock.Object);

            // Assert
            Assert.Equal(expectedLastUpdated, note.LastUpdateTime);
        }
    }
}
