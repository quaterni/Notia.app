using AutoMapper;
using Np.NotesService.Api.Controllers.Note;
using Np.NotesService.Application.Notes.AddNote;
using Np.NotesService.Application.Notes.GetNote;
using Np.NotesService.Application.Notes.UpdateNote;

namespace Np.NotesService.Api.Profiles
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<GetNoteResponse, NoteResponse>();
            CreateMap<AddNoteRequest, AddNoteCommand>();
        }
    }
}
