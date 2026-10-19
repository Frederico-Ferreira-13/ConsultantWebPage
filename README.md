
# ConsultantWebPage

### A Modern Full-Stack Real Estate Web Platform

**React | JavaScript | ASP.NET Core | C# | Entity Framework Core | SQL Server**

> 🚧 **Project Status: Active Development & Refactoring**
>
> ConsultantWebPage is an ongoing personal software development project. The application is being restructured to improve its architecture, maintainability, performance, and user experience.
>
> Several features are implemented at code level, while integration, validation, and production-readiness improvements remain in progress.

---

## 1. Project Overview

ConsultantWebPage is a Full-Stack web application designed to provide a modern digital experience for real estate consultants and their clients.

The project combines a React frontend with an ASP.NET Core Web API and a SQL Server database.

Its main purpose is to explore how modern software engineering can improve real estate workflows, property presentation, customer interactions, and internal business operations.

One of the project's distinguishing features is a property discovery interface inspired by streaming platforms such as Netflix, incorporating interactive property cards and video previews.

The application is also an opportunity to apply my previous experience in commercial management, CRM workflows, and real estate operations to practical software development challenges.

### Main Objectives

- Build a modern and responsive real estate platform.
- Improve property discovery through interactive interfaces.
- Explore video-first property presentation.
- Support client and consultant interactions.
- Organize business logic through a layered backend architecture.
- Implement authentication and role-based access control.
- Improve code maintainability through progressive refactoring.
- Develop practical Full-Stack engineering skills.

---

## 2. Technology Stack

### Frontend

| Technology | Purpose |
|---|---|
| React | Component-based user interfaces |
| JavaScript | Frontend application logic |
| Vite | Development tooling and bundling |
| React Router | Client-side routing and navigation |
| Axios / Fetch API | HTTP communication with the backend |
| HTML5 / CSS3 | Interface structure and styling |
| Tailwind CSS | Styling integration under review |

### Backend

| Technology | Purpose |
|---|---|
| C# | Backend programming language |
| ASP.NET Core (.NET 9) | REST API development |
| Entity Framework Core | Object-relational mapping |
| SQL Server | Relational database |
| JWT Authentication | Token-based authentication |
| BCrypt | Password hashing |
| Swagger / OpenAPI | API documentation and exploration |
| Dependency Injection | Service composition |
| Repository Pattern | Data-access abstraction |
| Unit of Work | Coordination of repository operations |

### External Services

- **Cloudinary:** Image and video upload integration.
- **YouTube / Vimeo:** Support for externally hosted property videos.

Some dependencies and integrations are currently undergoing review as part of the restructuring process.

---

## 3. Main Features

### 3.1 Property Discovery

The project includes interfaces and API endpoints for managing and displaying real estate listings.

Implemented components and endpoints cover:

- Property listings and detailed property pages.
- Search by location and selected property criteria.
- Property categories and types.
- Property images and related media.
- Featured property presentation.
- Responsive property cards.
- Property information and pricing display.

### 3.2 Netflix-Inspired Property Experience

A central UI concept is an interactive property catalogue inspired by streaming platforms.

The `PropertyCard` component supports:

- Interactive property cards.
- Hover animations and visual effects.
- Video previews.
- YouTube and Vimeo integration.
- Direct video playback.
- Full-screen-style video presentation through modal interfaces.
- Property information displayed alongside media content.

The objective is to provide a more engaging property browsing experience.

### 3.3 Authentication and Access Control

The project includes authentication components in both the frontend and backend.

The implementation covers:

- User login.
- JWT token generation and validation.
- Protected frontend routes.
- Role-based access restrictions.
- Client and administration areas.
- Password hashing services.

Authentication security, token handling, authorization rules, and end-to-end validation remain part of the ongoing development process.

### 3.4 Client Area

The frontend contains dedicated components for authenticated users, including:

- Client dashboard navigation.
- Favorite properties.
- User settings.
- Property evaluation requests.
- Market-related information interfaces.
- DISC assessment interface.

Some workflows remain under integration or use demonstration data.

### 3.5 Administration Area

The application includes an administration interface with components for:

- Property management.
- Content and article management.
- Testimonials.
- Profile content.
- Administrative overview.
- User-related operations.
- Lead-management functionality under development.

The administration area is being progressively refined and validated.

### 3.6 Property Evaluation

The project includes a property evaluation workflow.

The frontend currently supports an illustrative estimate calculated using predefined price ranges, property characteristics, and adjustment factors.

It also contains a form for submitting property evaluation requests to the backend.

**Important:** The displayed estimates are demonstration calculations, not certified valuations or live market predictions.

### 3.7 Media Management

The backend includes Cloudinary integration for image and video uploads.

The implementation covers:

- Image uploads.
- Video uploads.
- Media-type validation.
- File-size restrictions.
- External media URLs.

A valid Cloudinary configuration is required to use the corresponding upload functionality.

---

## 4. Software Architecture

The backend is organized into separate projects to improve responsibility separation and maintainability.

```text
ConsultantWebPage/
│
├── ConsultantWebPage.API/
│   ├── Controllers/
│   ├── Properties/
│   └── Program.cs
│
├── ConsultantWebPage.Core/
│   ├── Common/
│   ├── DTOs/
│   └── Models/
│
├── ConsultantWebPage.Contracts/
│   └── Interfaces/
│       ├── Repo/
│       └── Services/
│
├── ConsultantWebPage.Services/
│   ├── Import/
│   └── Business Services
│
├── ConsultantWebPage.Repo/
│   ├── ApplicationDbContext.cs
│   ├── Repositories
│   ├── UnitOfWork.cs
│   └── Migrations/
│
├── frontend/
│   ├── public/
│   ├── src/
│   │   ├── assets/
│   │   ├── components/
│   │   ├── config/
│   │   ├── context/
│   │   ├── pages/
│   │   └── services/
│   ├── package.json
│   └── vite.config.js
│
├── ConsultantWebPage.sln
└── ConsultantWebPage.slnx
```

### API Layer

Responsible for exposing HTTP endpoints, handling requests, and integrating the application services with ASP.NET Core.

### Core Layer

Contains domain entities, value objects, shared result types, error definitions, and supporting business concepts.

### Contracts Layer

Defines interfaces for repositories and services, supporting abstraction and dependency injection.

### Services Layer

Implements application and business operations, including authentication, property management, user operations, content management, and media integration.

### Repository Layer

Handles database access using Entity Framework Core, repository implementations, Unit of Work, database configuration, and migrations.

### Frontend

Provides the client-facing React application, including routing, reusable components, authentication context, API communication, public pages, and protected areas.

---

## 5. API Overview

The backend exposes REST endpoints for the main application resources.

Examples include:

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/RealEstate` | Retrieve active properties |
| GET | `/api/RealEstate/{id}` | Retrieve property details |
| GET | `/api/RealEstate/search` | Search properties |
| POST | `/api/Auth/login` | Authenticate a user |
| GET | `/api/Auth/me` | Retrieve the authenticated user |
| POST | `/api/MediaUpload/image` | Upload an image |
| POST | `/api/MediaUpload/video` | Upload a video |

Some endpoints require authentication or specific roles.

Swagger is configured for API exploration in the development environment.

---

## 6. Getting Started

### Prerequisites

The development environment requires:

- .NET 9 SDK
- Node.js and npm compatible with the installed Vite version
- SQL Server or SQL Server Express
- Git
- An IDE such as Visual Studio or Visual Studio Code

The project is currently under refactoring. Additional dependency adjustments may be necessary before all components build and run successfully.

### Step 1 — Clone the Repository

```bash
git clone https://github.com/Frederico-Ferreira-13/ConsultantWebPage.git
cd ConsultantWebPage
```

### Step 2 — Restore Backend Dependencies

```bash
dotnet restore ConsultantWebPage.sln
dotnet build ConsultantWebPage.sln
```

### Step 3 — Configure the Database

The backend uses SQL Server through Entity Framework Core.

Configure `ConnectionStrings:DefaultConnection` for your local database.

For development, use .NET User Secrets instead of committing credentials to source control.

From the repository root:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<YOUR_SQL_SERVER_CONNECTION_STRING>" --project ConsultantWebPage.API
```

Replace the placeholder with your own development connection string.

### Step 4 — Configure JWT Authentication

Generate a strong private signing key and configure it locally:

```bash
dotnet user-secrets set "JwtSettings:SecurityKey" "<YOUR_STRONG_PRIVATE_SIGNING_KEY>" --project ConsultantWebPage.API
```

The development settings also define the JWT issuer, audience, and token expiration.

Never commit signing keys, passwords, or production credentials to the repository.

### Step 5 — Apply Database Migrations

With the database configured and the EF Core CLI available, run:

```bash
cd ConsultantWebPage.API

dotnet ef database update --project ../ConsultantWebPage.Repo --startup-project .
```

The project includes Entity Framework Core migrations.

Migration execution depends on a valid database connection and compatible tooling.

### Step 6 — Run the Backend

From the API project directory:

```bash
dotnet run --launch-profile http
```

The current HTTP development profile uses:

```text
http://localhost:5170
```

When running successfully in Development mode, Swagger should be accessible at:

```text
http://localhost:5170/swagger
```

### Step 7 — Configure and Start the Frontend

Open another terminal from the repository root:

```bash
cd frontend

npm install

npm run dev
```

Vite normally starts the frontend on:

```text
http://localhost:5173
```

The frontend supports the `VITE_API_BASE_URL` environment variable.

For the current local HTTP configuration:

```env
VITE_API_BASE_URL=http://localhost:5170/api
```

The API currently allows local frontend origins on ports `5173` and `5174`.

**Development note:** The Vite configuration references `@tailwindcss/vite`, but this dependency is not declared in the current frontend `package.json`. The dependency configuration must be aligned if the frontend build reports a module resolution error.

---

## 7. Configuration and Security

The application uses environment-specific configuration and .NET User Secrets for sensitive backend values.

The current configuration includes:

- SQL Server connection settings.
- JWT authentication settings.
- Cloudinary credentials.
- Local frontend API configuration.
- CORS settings.

Cloudinary credentials must be configured before using the corresponding media-upload features.

The frontend currently uses browser local storage for authentication information. Token storage, authorization boundaries, input validation, and other security-sensitive workflows require further review before production use.

**Security principle:** Never commit secrets or real customer data to a public repository.

---

## 8. Current Development Status

ConsultantWebPage is undergoing a progressive restructuring process.

The repository includes the foundation for property management, client interactions, protected areas, administration, and media-based property presentation.

However, the presence of components or endpoints does not guarantee that every workflow has been fully integrated or tested.

### Areas Under Development

- Frontend component refactoring.
- Backend architecture improvements.
- Dependency and package management.
- End-to-end feature integration.
- Authentication and authorization review.
- Responsive design improvements.
- API error handling.
- Validation of property-management operations.
- Database consistency checks.
- Automated testing.

### Demonstration Features

Some components currently rely on illustrative data or incomplete integrations.

For example:

- The market dashboard displays predefined sample values.
- The property evaluation estimate uses static reference ranges.
- Property import includes a mock provider rather than a completed external platform integration.
- Some administration and CRM components are not yet integrated into active application routes.

These limitations are being addressed as the application evolves.

---

## 9. Roadmap

### Phase 1 — Architecture and Stability

- [ ] Review and stabilize backend dependencies.
- [ ] Align frontend dependencies and build configuration.
- [ ] Improve service and repository consistency.
- [ ] Review authentication and role-based permissions.
- [ ] Improve API error handling.
- [ ] Validate database operations and migrations.

### Phase 2 — Frontend Modernization

- [ ] Refactor reusable React components.
- [ ] Improve responsive behavior.
- [ ] Refine navigation and user interactions.
- [ ] Improve the property catalogue experience.
- [ ] Enhance video preview performance.
- [ ] Add loading, empty, and error states.

### Phase 3 — Business Features

- [ ] Complete property administration workflows.
- [ ] Improve client and consultant dashboards.
- [ ] Expand lead-management functionality.
- [ ] Improve property evaluation workflows.
- [ ] Develop supported property-import integrations.
- [ ] Replace demonstration market values with validated data sources.

### Phase 4 — Quality and Delivery

- [ ] Implement automated unit tests.
- [ ] Add integration tests for critical API workflows.
- [ ] Review application security.
- [ ] Validate complete user journeys.
- [ ] Add technical documentation and screenshots.
- [ ] Prepare a stable demonstration release.
- [ ] Define a deployment strategy.

---

## 10. Engineering Challenges and Learning

This project has provided practical experience in several areas of software engineering.

### Frontend and Backend Integration

Connecting a React application to an ASP.NET Core API requires careful handling of HTTP communication, authentication, routing, and application state.

### Layered Architecture

Separating API controllers, domain models, interfaces, services, and repositories has helped me explore maintainable software structures and improve responsibility separation.

### Database Design

Working with Entity Framework Core and SQL Server has provided experience with relational data modeling, entity relationships, queries, and database migrations.

### Authentication and Authorization

Implementing JWT-based authentication and role-based restrictions has introduced important considerations around application security and user access.

### UI/UX Development

Designing interactive property cards, video previews, and responsive interfaces has helped me strengthen my React and frontend development skills.

### Refactoring

A major objective of the current development phase is improving existing code rather than simply adding new features.

This includes reviewing architectural decisions, reducing unnecessary complexity, improving readability, and preparing the application for future expansion.

---

## 11. Screenshots and Demonstration

Application screenshots and a stable demonstration are planned for a future update.

The repository currently focuses on source code and ongoing development.

Visual examples will be added as individual interfaces and workflows are validated.

---

## 12. About the Developer

**Frederico Ferreira**

Junior Software Developer focused on C#, .NET, ASP.NET Core, React, and relational databases.

Before transitioning into software development, I worked in commercial management, team leadership, and real estate business operations.

My objective is to combine this professional background with software engineering skills to create practical applications that address real business needs.

ConsultantWebPage reflects this combination of business experience, technical learning, and continuous improvement.

### Connect With Me

- **GitHub:** [Frederico-Ferreira-13](https://github.com/Frederico-Ferreira-13)
- **LinkedIn:** [Frederico Ferreira](https://www.linkedin.com/in/frederico-ferreira-dev/)

---

## 13. License

No license has been selected for this repository.

Until a license is explicitly added, no permission for redistribution or modification should be assumed.

---

**Built with C#, ASP.NET Core, React, and a commitment to continuous learning.**

*An ongoing journey from business experience to software development.*
