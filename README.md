# Rosa — Full-Stack E-Commerce Platform

A full-stack e-commerce web application built with ASP.NET Core and Next.js, featuring real-time order tracking, Stripe payment integration, and containerized deployment on AWS.

## Live Demo

🔗 [http://rosa.com](http://44.214.67.80:3000)

**Test Card:** `4242 4242 4242 4242` | Expiry: any future date | CVC: any 3 digits

## Tech Stack

### Backend

- **Runtime:** ASP.NET Core (.NET 10) / C#
- **Database:** MySQL 8 with Entity Framework Core
- **Auth:** JWT (httpOnly cookies) with cookie-based authentication
- **Payments:** Stripe.NET — PaymentIntents, webhooks
- **Real-time:** SignalR WebSocket for live order status updates
- **Architecture:** RESTful API, Repository/Service pattern

### Frontend

- **Framework:** Next.js 16 (App Router, Turbopack)
- **Language:** TypeScript
- **Styling:** Tailwind CSS
- **State:** Zustand
- **HTTP Client:** Axios with cookie credentials
- **Payments:** @stripe/react-stripe-js, Stripe PaymentElement

### DevOps

- **Containerization:** Docker with multi-stage builds
- **Orchestration:** Docker Compose (MySQL + Backend + Frontend)
- **CI/CD:** GitHub Actions — build, test, deploy on push to main
- **Cloud:** AWS EC2, Elastic IP
- **Web Server:** Node.js standalone (Next.js), Kestrel (ASP.NET)

## Features

- **Product Catalog** — Browse, search, filter by category, sort by price/date, load more pagination
- **Search** — Debounced autocomplete with arrow key navigation
- **Shopping Cart** — Add, update quantity, remove items, real-time badge sync
- **Checkout** — Stripe PaymentElement with secure card processing
- **Order Management** — Real-time status updates via SignalR WebSocket
- **Authentication** — Register, login, logout with httpOnly cookie JWT
- **Profile** — Edit name, email, change password
- **Protected Routes** — Auth-guarded pages with loading states

## Project Structure

```
rosa/
├── .github/workflows/ci.yml    # CI/CD pipeline
├── docker-compose.yml           # Container orchestration
├── backend/
│   ├── Controllers/             # API endpoints
│   ├── Services/                # Business logic
│   ├── Models/                  # EF Core entities
│   ├── DTOs/                    # Request/Response objects
│   ├── Data/                    # DbContext
│   ├── Hubs/                    # SignalR WebSocket
│   ├── Migrations/              # Database migrations
│   └── Dockerfile
├── frontend/
│   ├── src/
│   │   ├── app/                 # Next.js pages (App Router)
│   │   ├── components/          # Reusable UI components
│   │   ├── hooks/               # Custom hooks (useAuth, useCart, useProducts)
│   │   ├── store/               # Zustand state management
│   │   ├── lib/                 # Axios config
│   │   └── types/               # TypeScript interfaces
│   └── Dockerfile
```

## API Endpoints

### Auth

| Method | Endpoint       | Description               |
| ------ | -------------- | ------------------------- |
| POST   | /auth/register | Register new user         |
| POST   | /auth/login    | Login with email/password |
| POST   | /auth/logout   | Logout (clear cookie)     |

### Products

| Method | Endpoint       | Description                                    |
| ------ | -------------- | ---------------------------------------------- |
| GET    | /products      | List products (search, filter, sort, paginate) |
| GET    | /products/{id} | Get product details                            |

### Cart

| Method | Endpoint   | Description          |
| ------ | ---------- | -------------------- |
| GET    | /cart      | Get user's cart      |
| POST   | /cart      | Add item to cart     |
| PUT    | /cart/{id} | Update item quantity |
| DELETE | /cart/{id} | Remove item          |
| DELETE | /cart      | Clear cart           |

### Orders

| Method | Endpoint | Description            |
| ------ | -------- | ---------------------- |
| GET    | /orders  | Get user's orders      |
| POST   | /orders  | Create order from cart |

### Payments

| Method | Endpoint           | Description                 |
| ------ | ------------------ | --------------------------- |
| POST   | /payment/{orderId} | Create Stripe PaymentIntent |
| POST   | /payment/webhook   | Stripe webhook handler      |

## Getting Started

### Prerequisites

- Docker & Docker Compose
- Node.js 22+
- .NET 10 SDK

### Local Development

```bash
# Clone
git clone https://github.com/YOUR_USERNAME/rosa.git
cd rosa

# Backend
cd backend
dotnet run

# Frontend (new terminal)
cd frontend
npm install
npm run dev
```

### Docker

```bash
# Create .env in project root
cp .env.example .env
# Edit .env with your secrets

# Run everything
docker compose up --build -d

# Seed database
docker exec -it rosa-mysql mysql -u root -padmin123 -e "
USE rosa_db;
INSERT INTO Categories (CategoryName) VALUES
('Electronics'), ('Furnitures'), ('Clothing'), ('Books'), ('Sports');
"
```

## Environment Variables

### Root `.env` (Docker)

```
JWT_SECRET=your-jwt-secret
STRIPE_SECRET_KEY=sk_test_...
STRIPE_PUBLISHABLE_KEY=pk_test_...
STRIPE_WEBHOOK_SECRET=whsec_...
```

### Frontend `.env.local`

```
NEXT_PUBLIC_API_URL=http://localhost:5130
```

## Architecture

```
                    ┌──────────────────┐
                    │     Browser      │
                    └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
                    │   Next.js :3000  │
                    │   (Frontend)     │
                    └────────┬─────────┘
                             │
                    ┌────────▼─────────┐
              ┌─────┤  ASP.NET :5130   ├─────┐
              │     │   (Backend)      │     │
              │     └────────┬─────────┘     │
              │              │               │
     ┌────────▼───┐   ┌──────▼──────┐  ┌─────▼─────┐
     │  SignalR   │   │ MySQL :3306 │  │  Stripe   │
     │  WebSocket │   │ (Database)  │  │ (Payments)│
     └────────────┘   └─────────────┘  └───────────┘
```

## Deployment

Deployed on AWS EC2 with Docker Compose. CI/CD via GitHub Actions automatically builds and deploys on push to main.

```
Push to main → GitHub Actions → SSH into EC2 → git pull → docker compose up
```
