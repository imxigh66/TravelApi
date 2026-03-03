Digital Tourism Platform

Digital Platform for Experience Sharing in the Tourism Industry

Project Title

Digital Platform for Experience Sharing in the Tourism Industry
(Цифровая платформа для обмена опытом в сфере туризма)

Project Goal

The goal of this project is to design and develop a social platform for travelers that integrates social networking, collaborative trip planning, and AI-based recommendations into a single system.

The system allows users to share travel experiences, create and edit routes collaboratively, communicate in real time, and receive personalized recommendations.

Technologies and Tools
Backend

.NET 8

ASP.NET Core Web API

Entity Framework Core (Code First)

MS SQL Server

MediatR (CQRS)

FluentValidation

JWT Authentication + Refresh Tokens

SignalR

AWS S3

OpenAI API

Architecture

Clean Architecture (4 layers)

CQRS pattern

RESTful API

Swagger / OpenAPI

Frontend

React.js

Axios

React Router

Mapbox

SignalR client

How to Run
Backend

Clone repository:

git clone <repository_url>

Configure connection string in appsettings.json.

Apply migrations:

dotnet ef database update

Run project:

dotnet run

Swagger:

https://localhost:5001/swagger
Frontend
cd client
npm install
npm start
Application Structure

The system follows Clean Architecture and consists of four layers:

Domain – core entities and business models.
Application – business logic, CQRS handlers, DTOs, validation.
Infrastructure – EF Core, external services (S3, SMTP, SignalR, OpenAI).
WebAPI – controllers and application entry point.

Key Features

Social feed with posts and comments

Place catalog with ratings and tags

Trip planner with daily organization

Collaborative route editing

Real-time messaging

AI-based recommendations and route optimization

Business account verification

Route export (PDF / Calendar)
