using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes.Events;
using System.Text.RegularExpressions;

namespace Np.NotesService.Domain.Notes
{
    public class Note : Entity
    {
        /// <summary>
        /// Max amount characters of the title
        /// </summary>
        public const int TitleMaxLength = 400;

        private Note() : base()
        {
        }

        protected Note(string title, string content, Guid id, DateTime createdTime, DateTime lastUpdateTime) : base(id) 
        {
            Title = title;
            Content = content;
            CreateTime = createdTime;
            LastUpdateTime = lastUpdateTime;
        }

        /// <summary >
        /// Represents substring of the first line or the first characters of the content 
        /// </summary>
        public string Title { get; private set; } = null!;

        public string Content { get; private set; } = null!;

        /// <summary>
        /// Date of note creation (utc)
        /// </summary>
        public DateTime CreateTime { get; }

        /// <summary>
        /// Date of the last content update (utc)
        /// </summary>
        public DateTime LastUpdateTime { get; private set; }

        /// <summary>
        /// Create new note
        /// </summary>
        /// <param name="data">Data that added to the content and substring set to the title</param>
        /// <param name="dateTimeProvider">Provides current utc time</param>
        /// <returns>New note with own id</returns>
        public static Note Create(string data, IDateTimeProvider dateTimeProvider)
        {
            var currentTime = dateTimeProvider.GetCurrentTime();

            data = data.TrimStart();

            var title = GetTitleFromData(data);

            var id = Guid.NewGuid();
            var note = new Note(title, data, id, currentTime, currentTime);

            note.AddDomainEvent(new NoteCreatedEvent(id));

            return note;
        }

        /// <summary>
        /// Update note data
        /// </summary>
        /// <remarks>
        /// Set the LastUpdateTimeUtc from <see cref="dateTimeProvider"/> 
        /// <br/>
        /// Result always successful
        /// </remarks>       
        /// <param name="data">Data that added to the content and substring set to the title</param>
        /// <param name="dateTimeProvider">Provides current utc time</param>
        public Result UpdateNote(string data, IDateTimeProvider dateTimeProvider)
        {
            data = data.TrimStart();
            var title = GetTitleFromData(data);
            Title = title;
            Content = data;

            LastUpdateTime = dateTimeProvider.GetCurrentTime();

            AddDomainEvent(new NoteChangedEvent(Id));

            return Result.Success();
        }

        private static string GetTitleFromData(string data)
        {
            var title = data.Substring(0, data.Length < TitleMaxLength ? data.Length : TitleMaxLength);

            var match = Regex.Matches(title, @"^(.*)\w*\n");

            if (match.Count > 0)
            {
                title = match.First().Value;
            }

            return title.Trim();
        }
    }
}
