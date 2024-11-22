
using System.Collections;

namespace Np.NotesService.Domain.Tests.Notes
{
    public class NoteTestData : IEnumerable<object[]>
    {
        public static NoteTestDto OneLineNote => new("One row data", "One row data", "One row data");

        public static NoteTestDto TitleAndContentBetweenTwoEmptyLineNote =>
            new("Title", 
                @"Title
          

Content",
                @"Title
          

Content");

        public static NoteTestDto TwoLinesNote => new(
            "Title", 
            @"Title
Content",
            @"Title
Content");

        public static NoteTestDto MaxTitleLengthWithoutContentNote => new(
            "TitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitle",
            "TitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitle",
            "TitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitle");

        public static NoteTestDto MaxTitleLengthWithContentNote = new("TitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitle",
            "TitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleContent",
            "TitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleTitleContent");

        public static NoteTestDto EmptyLinesBeforeTitle = new(
            @"Title",
            @"Title",
            @"

Title");

        public static NoteTestDto EmptyLinesAfterTitle = new(
    @"Title",
    @"Title

",
    @"Title

");

        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                OneLineNote
            };

            yield return new object[]
            {
                TitleAndContentBetweenTwoEmptyLineNote
            };
            yield return new object[]
            {
                TwoLinesNote
            };

            yield return new object[]
            {
                MaxTitleLengthWithoutContentNote
            };
            yield return new object[]
            {
                MaxTitleLengthWithContentNote
            };
            yield return new object[]
            {
                EmptyLinesBeforeTitle
            };
            yield return new object[]
            {
                EmptyLinesAfterTitle
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    }
}
