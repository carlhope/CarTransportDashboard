# 🚗 Car Transport Dashboard
A full‑stack transport‑operations dashboard built with Angular, .NET 9, RabbitMQ, and Google Maps / Routes APIs. It models the real workflow of a transport job — creation, routing, pricing, and notifications — with a modular, maintainable architecture.

<img width="3024" height="1964" alt="Image 24-01-2026 at 23 05" src="https://github.com/user-attachments/assets/6101df5d-82b6-49dd-9b23-7215ac94da23" />

## 🔧 Tech Stack
Frontend: Angular 20+, TypeScript, Tailwind CSS

Backend: .NET Core Web API, RabbitMQ, Entity Framework

## Authentication: 
uses a modern, production‑style security setup that includes short‑lived JWT access tokens with rotating refresh tokens, fully enforced role‑based authorization across both the API and Angular UI, CSRF protection to prevent cross‑site request forgery, secure cookie handling with HttpOnly and SameSite rules where appropriate, and a combination of Angular route guards and backend authorization policies to ensure that only the correct roles can access protected routes or perform sensitive actions.

## Clone repository: 
```bash
git clone https://github.com/carlhope/CarTransportDashboard.git
cd CarTransportDashboard
```

## Install Prerequisites
Node 18.19+

Angular CLI

.NET 9 SDK

SQL Server (optional; only needed if you switch from the default InMemory database)

Docker Desktop (required for RabbitMQ)

Google Cloud APIs (Maps JS, Routes, Geocoding)

Google OAuth credentials (for your own login flow)

## Configure Secrets
In CarTransportDashboard.Web, copy localSecrets.Example.ts to     localSecrets.ts. Add your Google Maps JS API key.

In CarTransportDashboard, Update user secrets to include your Google Routes API key under "GoogleMaps:ApiKey". 

## Configure Google OAuth
Create Google OAuth credentials in Google Cloud Console, and update client secret, currently in src/app/pages/auth/login/login.ts

## Update database connection
Project currently utilises InMemory Database. Should you wish to use an alternative, update program.cs and run migrations.

## Start RabbitMQ
```bash
docker run -d \
  --hostname rabbitmq \
  --name rabbitmq \
  -p 5672:5672 \
  -p 15672:15672 \
  rabbitmq:3-management
```
## Run the projects
```bash
cd CarTransportDashboard
dotnet run
cd ../EmailConsumer
dotnet run
cd ../CarTransportDashboard.Web
npm install
ng serve
```
