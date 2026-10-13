# RaceDay API Endpoint Plan

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
| :--- | :--- | :--- | :--- | :--- | :--- |
| POST | /api/auth/register | Register a new user | None | { email, password, role, fullName, phoneNumber } | 201 Created or 400 Bad Request |
| POST | /api/auth/login | Login and get token | None | { email, password } | 200 OK with token or 401 Unauthorized |
| GET | /api/profile | Get user profile | Any logged in user | None | 200 OK with profile or 404 Not Found |
| GET | /api/events | Get all events | None | None | 200 OK with list of events |
| GET | /api/events/{id} | Get single event | None | None | 200 OK with event or 404 Not Found |
| POST | /api/events | Create event | Organiser | { title, description, eventDate, location, maxParticipants } | 201 Created or 400 Bad Request or 403 Forbidden |
| PUT | /api/events/{id} | Update event | Organiser (owner) | { title, description, eventDate, location } | 204 No Content or 403 Forbidden or 404 Not Found |
| DELETE | /api/events/{id} | Delete event | Organiser (owner) | None | 204 No Content or 403 Forbidden or 404 Not Found |
| GET | /api/events/{id}/categories | Get event categories | Any logged in user | None | 200 OK with list or 404 Not Found |
| POST | /api/events/{id}/categories | Add category | Organiser (owner) | { name, distance, entryFee, startTime, maxParticipants } | 201 Created or 403 Forbidden or 400 Bad Request |
| POST | /api/enrolments | Enter an event | Participant | { eventId, categoryId } | 201 Created or 409 Conflict or 400 Bad Request |
| GET | /api/enrolments/me | Get my enrolments | Participant | None | 200 OK with list of enrolments |
| POST | /api/results | Capture result | Organiser (owner) | { enrolmentId, finishTime, overallPosition, categoryPosition } | 201 Created or 400 Bad Request or 404 Not Found |
| GET | /api/results/me | Get my results | Participant | None | 200 OK with list of results |
