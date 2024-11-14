using Dapper;
using Np.NotesService.Application.Abstractions.Data;
using Np.NotesService.Application.Abstractions.Mediator;
using Np.NotesService.Domain.Abstractions;
using Np.NotesService.Domain.Notes;

namespace Np.NotesService.Application.Notes.GetNote
{
    internal class GetNoteQueryHandler : IQueryHandler<GetNoteQuery, GetNoteResponse>
    {
        private readonly ISqlConnectionFactory _sqlConnectionFactory;

        public GetNoteQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
        {
            _sqlConnectionFactory = sqlConnectionFactory;
        }

        public async Task<Result<GetNoteResponse>> Handle(GetNoteQuery request, CancellationToken cancellationToken)
        {
            using var connection = _sqlConnectionFactory.CreateConnection();

            var result = await connection
                .QueryFirstOrDefaultAsync<dynamic>("SELECT * FROM notes WHERE id=@Id", new { request.Id });

            if (result == null)
            {
                return Result.Failure<GetNoteResponse>(GetNoteErrors.NotFound);
            }

            if(!((Guid)result.user_id).Equals(request.UserId!.Value))
            {
                return Result.Failure<GetNoteResponse>(GetNoteErrors.NotFound);
            }

            var note = new GetNoteResponse
            {
                Content = result.content,
                Title = result.title,
                Id = result.id,
                CreateTime = result.create_time,
                LastUpdateTime = result.last_update_time,
            };

            return note;
        }
    }
}
