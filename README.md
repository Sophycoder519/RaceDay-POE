A full-stack platform for South African road running, walking, and cycling events.

## Roles
- **Organiser**: Create/manage events, categories, capture results, view enrolments
- **Participant**: Browse events, enter events, view personal results

## Project Structure
RaceDay-POE/
├── RaceDayAPI/ # Part 2 - RESTful API (ASP.NET Core 8)
│ ├── Controllers/ # API endpoints
│ ├── Models/ # EF Core entities
│ ├── DTOs/ # Data transfer objects
│ ├── Services/ # JwtService
│ ├── Data/ # DbContext
│ └── Migrations/ # EF Core migrations
├── RaceDayAPI.Tests/ # xUnit unit tests
├── docs/ # Part 1 planning documents
│ ├── erd.pdf
│ ├── endpoint-plan.md
│ └── script.sql
└── .github/workflows/ # CI/CD pipeline



## Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server or LocalDB

### Running the API
```bash
cd RaceDayAPI
dotnet restore
dotnet ef database update
dotnet run
Open browser to: http://localhost:5211/swagger

Running Tests
bash
cd RaceDayAPI
dotnet test
Authentication
The API uses JWT Bearer tokens. Register or login via /api/Auth/register and /api/Auth/login to receive a token. Use the token in the Authorization: Bearer {token} header for protected endpoints.

CI/CD Status
https://part2-ci-screenshot.png/

The GitHub Actions workflow builds the API and runs 10 unit tests on every push.

Video Presentation
Watch Part 2 walkthrough

AI Disclosure
This project was developed with AI assistance for structuring and debugging. All final code was reviewed, tested, and implemented by me.

