np is the backend of a hypertext note application built on a microservice architecture
### How it works
You can create a note with content, content is any string data, first line or the first 400 characters of the content is note title. Notes can be linked to each other. A note that links to another note is an outgoing note and a linked note is an incoming note.

The actual application implements a REST API for notes and relations
### In Progress
`v0.2`
- Create user authentication with tokens
### Start up
Services must be configured before they can be started. Empty configurations can be found in the `appsetting.Empty.json` file, copy the contents to the `appsetting.json` file and fill in. The following describes requirements to configure each service.

Notes service:
- connection string of database with applied EF Core Migrations
- RabbitMQ options: connection and exchange
- gRPC endpoint of relations service
- outbox options

Relations service:
- connection string of database with applied EF Core Migrations
- Kestrel options: http and grpc endpoints
- RabbitMQ options: connection, exchange and queue
- outbox options

Easiest way run application from docker compose file `Deployment/compose.yaml`. Each service also contains dockerfile.
### Endpoints
#### Notes service endpoints
- POST `api/notes` - create note
Request sample:
``` json
{
  "data" : "C#\nC# is object-oriented programming language."
}
```

- GET `api/notes/{id}` - get note, where `{id}` is note's GUID
Output sample:
``` json
{
  "title": "C#",
  "content": "C#\nC# is object-oriented programming language.",
  "createTime": "2024-10-15T07:09:45.644355Z",
  "lastUpdateTime": "2024-10-15T07:09:45.644355Z",
  "id": "0c0d856d-e94f-4094-a0f0-f9c7477ae16c"
}
```

- PUT `api/notes/{id}` - update note content, where `{id}` is note's GUID
Request sample:
``` json
{
  "data" : "C#\nC# is a general-purpose high-level programming language supporting multiple paradigms."
}
```

- DELETE `api/notes/{id}` - delete note, where `{id}` is note's GUID

- GET `api/notes/root` - get notes that not outgoing
Output sample:
``` json
[
  {
    "title": "C#",
    "id": "ce3b7d25-eca5-4c8c-b7ac-4aff7e8a8067"
  },
  {
    "title": "Python",
    "id": "a72301f1-1fe2-48d1-abe7-ce49a58935e4"
  }
]
```

- GET `api/notes/{id}/incomings` - get list of relations where note is incoming, where `{id}` is note's GUID
Output sample:
``` json
[
    {
        "id": "478fc59d-0e21-4529-8682-9f12d56bcd8a",
        "outgoingNote": {
            "title": "C#",
            "id": "2e30da14-23b7-4f4b-afaf-49e718781288"
        },
        "incomingNote": {
            "title": "Programming languages",
            "id": "d37751fe-0dc0-4e38-ad61-dd2368bec47c"
        }
    },
    {
        "id": "b0e29e40-9bdc-4399-b31f-f946101b865d",
        "outgoingNote": {
            "title": "Python",
            "id": "47bdbc4a-d1a9-4099-bb61-9f3aa7100869"
        },
        "incomingNote": {
            "title": "Programming languages",
            "id": "d37751fe-0dc0-4e38-ad61-dd2368bec47c"
        }
    }
]
```

- GET `api/notes/{id}/outgoings` - get list of relations where note is outgoing, where `{id}` is note's GUID
Output sample:
``` json
[
    {
        "id": "478fc59d-0e21-4529-8682-9f12d56bcd8a",
        "outgoingNote": {
            "title": "C#",
            "id": "2e30da14-23b7-4f4b-afaf-49e718781288"
        },
        "incomingNote": {
            "title": "Programming languages",
            "id": "d37751fe-0dc0-4e38-ad61-dd2368bec47c"
        }
    },
    {
        "id": "b196f306-469b-4e21-998a-81aacad2408f",
        "outgoingNote": {
            "title": "C#",
            "id": "2e30da14-23b7-4f4b-afaf-49e718781288"
        },
        "incomingNote": {
            "title": ".NET",
            "id": "32e6d7e2-6cfc-434c-b183-27d8bbfeb3d2"
        }
    }
]
```

#### Relations service endpoints
- PUT `api/relations` - add new relation
Request sample
``` json
{
    "outgoingNoteId": "2e30da14-23b7-4f4b-afaf-49e718781288",
    "incomingNoteId": "d37751fe-0dc0-4e38-ad61-dd2368bec47c"
}
```
- DELETE `api/relations/{id}` - delete relation, where `{id}` is relation's GUID
