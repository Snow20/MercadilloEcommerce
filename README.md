Markdown# Feira Gallega Enterprise - Modular E-Commerce Architecture

A high-performance, enterprise-grade e-commerce platform built with a modern decoupled architecture: Angular 18 frontend with a custom Galician Market UI/UX design system, .NET 9 Minimal APIs backend, asynchronous event-driven messaging with MassTransit and RabbitMQ, a dedicated ServiceNow Incident Worker, Stripe Checkout payment gateway integration, and Infrastructure as Code (IaC) for Azure AKS deployment.

---

## Table of Contents
1. [English Version](#english-version)
   - [Project Overview](#project-overview)
   - [Architectural Blueprint](#architectural-blueprint)
   - [Tech Stack](#tech-stack)
   - [UI/UX Design System: Feira Gallega](#uiux-design-system-feira-gallega)
   - [Repository Structure](#repository-structure)
   - [Getting Started (Local Execution)](#getting-started-local-execution)
   - [Stripe Checkout & Webhook Integration](#stripe-checkout--webhook-integration)
   - [API Endpoints & Integration](#api-endpoints--integration)
   - [Infrastructure & Cloud Provisioning](#infrastructure--cloud-provisioning)
2. [Versión en Español](#versión-en-español)
   - [Visión General del Proyecto](#visión-general-del-proyecto)
   - [Arquitectura del Sistema](#arquitectura-del-sistema)
   - [Tecnologías Utilizadas](#tecnologías-utilizadas)
   - [Sistema de Diseño UI/UX: Feira Gallega](#sistema-de-diseño-uiux-feira-gallega)
   - [Estructura del Repositorio](#estructura-del-repositorio)
   - [Guía de Ejecución Local](#guía-de-ejecución-local)
   - [Integración de Stripe Checkout y Webhooks](#integración-de-stripe-checkout-y-webhooks)
   - [Endpoints de la API e Integración](#endpoints-de-la-api-e-integración)
   - [Infraestructura y Aprovisionamiento Cloud](#infraestructura-y-aprovisionamiento-cloud)
3. [License & Credits](#license--credits)

---

## English Version

### Project Overview
**Feira Gallega Enterprise** is a production-ready blueprint for decoupled e-commerce solutions. It features a modern SPA frontend built with Angular 18 that reflects the traditional yet modern Galician fair aesthetic, backed by a microservices-capable .NET 9 Web API. Secure card payments are handled seamlessly via **Stripe Checkout API** and verified asynchronously through signed webhooks. Critical system alerts and support requests are handled asynchronously through RabbitMQ and consumed by an automated worker that logs incidents into ServiceNow via REST APIs.

---

### Architectural Blueprint

                 [ Client / Web Browser ]
                            │
                            │ (HTTP / Port 80)
                            ▼
               [ NGINX Reverse Proxy ]
               ┌────────────┴────────────┐
               │                         │
        (Static Assets)          (API Proxy /api/*)
               │                         │
               ▼                         ▼
     [ Angular 18 Frontend ]    [ .NET 9 Core API ]
                                   │         │
                 (Create Checkout) │         │ (Publish Incident Event)
                                   ▼         ▼
                 [ Stripe Checkout ]    [ RabbitMQ Broker ]
                           │                 │
            (Signed Webhook)│                 │ (Consume Queue)
                           ▼                 ▼
             [ Webhook Endpoint ]    [ ServiceNow Worker ]
                                             │
                                     (Basic Auth REST)
                                             │
                                             ▼
                                  [ ServiceNow Instance ]

---

### Tech Stack

* **Frontend:** Angular 18 (Standalone Components, Signals, Reactive Forms, i18n support for GL, ES, EN, `@stripe/stripe-js`).
* **Reverse Proxy:** NGINX Alpine (Route mapping, CORS elimination, static asset hosting).
* **Backend API:** ASP.NET Core 9.0 (Minimal APIs, Stripe.net SDK, JWT Bearer Authentication, Swagger / OpenAPI).
* **Payment Gateway:** Stripe Checkout Hosted Sessions & Asynchronous Webhook Event Handlers (`checkout.session.completed`).
* **Event Messaging:** MassTransit + RabbitMQ (Asynchronous event bus and queue management).
* **Background Worker:** .NET 9 Worker Service (`ServiceNowWorker`) consuming incident messages.
* **Containers:** Multi-stage `Dockerfiles` orchestrated via `docker-compose`.
* **Infrastructure as Code (IaC):**
  * **Terraform:** Provisioning Azure Resource Groups, Virtual Networks, and AKS (Azure Kubernetes Service).
  * **Ansible:** Automated cluster configuration and Kubernetes workload manifest deployments.
  * **Python:** Automation pipeline runner (`deploy_azure.py`).

---

### UI/UX Design System: Feira Gallega

The design system incorporates regional Galician cultural themes while maintaining WCAG 2.1 accessibility compliance:

| Design Token | Color Hex | Usage |
| :--- | :--- | :--- |
| **Main Background** | `#F9F8F6` | Organic Linen / Warm Bone background |
| **Primary Brand** | `#0A2540` | Deep Marine Blue for headings, headers, and footer |
| **Traditional Accent**| `#C85A32` | Terracotta / Tello for badges, "Feira" labels, and highlights |
| **Primary Action (CTA)**| `#2D6A4F` | Organic Green for purchase buttons and active states |
| **Secondary Gray** | `#6B7280` | Auxiliary text, producer notes, and metadata |
| **Focus Ring** | `#80BFFF` | 3px accessible focus outline for keyboard navigation |

---

### Repository Structure

```text
.
├── docker-compose.yml          # Container orchestration for local environment
├── src/
│   ├── backend/                # ASP.NET Core 9.0 Minimal API
│   │   ├── Dockerfile
│   │   ├── EcommerceApi.csproj
│   │   ├── appsettings.json    # Stripe API keys & JWT configurations
│   │   └── Program.cs          # Minimal APIs, Stripe SDK & MassTransit setup
│   ├── frontend/               # Angular 18 SPA
│   │   ├── Dockerfile
│   │   ├── nginx.conf          # Reverse Proxy routing configuration
│   │   ├── src/
│   │   │   ├── main.ts         # Main component, signals, Stripe payment & HTTP requests
│   │   │   ├── styles.css      # Design Tokens & global CSS rules
│   │   │   └── app/
│   │   │       └── components/language-switcher/
│   │   └── package.json
│   └── workers/                # ServiceNow Background Worker
│       ├── Dockerfile
│       ├── Program.cs
│       └── ServiceNowWorker.csproj
└── infra/                      # Infrastructure as Code
    ├── terraform/              # Azure AKS provisioning
    ├── ansible/                # Kubernetes playbooks
    └── scripts/                # Automated deployment scripts
Getting Started (Local Execution)PrerequisitesDocker Engine 24+ & Docker Compose v2+GitStripe CLI (for local webhook testing)Execution StepsClone the repository:Bashgit clone [https://github.com/Snow20/Mercadillo-Ecommerce.git](https://github.com/Snow20/Mercadillo-Ecommerce.git)
cd Mercadillo-Ecommerce
Configure Stripe API keys in docker-compose.yml environment variables or appsettings.json:JSON"Stripe": {
  "SecretKey": "sk_test_...",
  "PublishableKey": "pk_test_...",
  "WebhookSecret": "whsec_..."
}
Build and start the container stack:Bashdocker-compose up --build -d
Verify running services:Frontend & E-Commerce Application: http://localhostBackend API Swagger Documentation: http://localhost:5000/swaggerRabbitMQ Management Dashboard: http://localhost:15672 (User: guest | Pass: guest)Stop local environment:Bashdocker-compose down
Stripe Checkout & Webhook IntegrationAccess the web interface at http://localhost.Click 💳 Pagar con Stripe on any regional product card (e.g., Polbo á Feira or Queixo Arzúa-Ulloa).The Angular client requests a session from .NET 9 API (/api/payment/create-checkout-session) and redirects the client securely to the Stripe Hosted Checkout Page.Test Credit Card Details:Card Number: 4242 4242 4242 4242Expiry Date: Any future date (e.g., 12/28)CVC: 123Local Webhook Event Forwarding (Stripe CLI):Bashstripe listen --events checkout.session.completed --forward-to localhost/api/payment/webhook
Trigger Manual Test Event:Bashstripe trigger checkout.session.completed
Expected Result: HTTP 200 OK returned by .NET 9 API with log output [Stripe Webhook OK]: Pago confirmado para la sesión cs_test_....API Endpoints & Integration1. Catalog EndpointGET /api/catalogDescription: Returns the complete list of 10 Galician artisanal products.Sample Response:JSON[
  { "id": 1, "name": "Polbo á Feira (Ración)", "price": 18.50 },
  { "id": 2, "name": "Queixo Arzúa-Ulloa DOP", "price": 9.20 }
]
2. Cart Order EndpointPOST /api/cartDescription: Receives purchase requests from the client.Payload:JSON{
  "productId": 1,
  "productName": "Polbo á Feira (Ración)",
  "price": 18.50
}
3. Stripe Checkout Session EndpointPOST /api/payment/create-checkout-sessionDescription: Generates a secure hosted Stripe Checkout session URL.Payload:JSON{
  "productId": 1,
  "productName": "Polbo á Feira (Ración)",
  "price": 18.50
}
Sample Response:JSON{
  "sessionId": "cs_test_a1b2c3...",
  "url": "[https://checkout.stripe.com/c/pay/cs_test_a1b2c3](https://checkout.stripe.com/c/pay/cs_test_a1b2c3)..."
}
4. Stripe Webhook Listener EndpointPOST /api/payment/webhookDescription: Consumes cryptographic Stripe webhooks, validates signature signatures (Stripe-Signature), and processes transaction completions.5. Incident Alert Endpoint (ServiceNow Integration)POST /api/incidentsDescription: Publishes an IncidentAlertEvent message to RabbitMQ. The servicenow-worker consumes the queue and creates an incident in ServiceNow.Payload:JSON{
  "title": "Incidencia en producto: Polbo á Feira",
  "description": "Alerta generada desde la web para el producto ID 1",
  "severity": "HIGH"
}
Infrastructure & Cloud ProvisioningTo deploy the platform to Azure AKS using IaC:Login to Azure CLI:Bashaz login
Run automated pipeline:Bashpython3 infra/scripts/deploy_azure.py
Versión en EspañolVisión General del ProyectoFeira Gallega Enterprise es una arquitectura de referencia para la construcción de plataformas e-commerce desacopladas. Cuenta con un frontend SPA en Angular 18 diseñado bajo la estética visual de una "Feira Gallega Moderna", respaldado por una API REST en .NET 9. Los pagos con tarjeta se procesan con total cumplimiento PCI-DSS mediante la API de Stripe Checkout y la ingesta asíncrona de Webhooks firmados. El sistema incluye un procesamiento de eventos con RabbitMQ para notificar incidencias técnicas en tiempo real a ServiceNow mediante un servicio Worker independiente.Arquitectura del Sistema             [ Cliente / Navegador Web ]
                        │
                        │ (HTTP / Puerto 80)
                        ▼
           [ NGINX Proxy Inverso ]
           ┌────────────┴────────────┐
           │                         │
    (Archivos Estáticos)      (Proxy API /api/*)
           │                         │
           ▼                         ▼
 [ Frontend Angular 18 ]    [ API Backend .NET 9 ]
                               │         │
             (Crear Checkout)  │         │ (Publica Evento Incidencia)
                               ▼         ▼
             [ Stripe Checkout ]    [ Broker RabbitMQ ]
                       │                 │
      (Webhook Firmado)│                 │ (Consume Cola)
                       ▼                 ▼
         [ Endpoint Webhook ]    [ Worker ServiceNow ]
                                         │
                                 (Basic Auth REST)
                                         │
                                         ▼
                              [ Instancia ServiceNow ]
Tecnologías UtilizadasFrontend: Angular 18 (Componentes Standalone, Signals, Formularios Reactivos, Selector multilingüe: GL, ES, EN, @stripe/stripe-js).Reverse Proxy: NGINX Alpine (Enrutamiento de red, eliminación de CORS, entrega de estáticos).Backend API: ASP.NET Core 9.0 (Minimal APIs, SDK Stripe.net, Autenticación JWT Bearer, Documentación Swagger / OpenAPI).Pasarela de Pagos: Sesiones alojadas Stripe Checkout y procesador asíncrono de eventos Webhook (checkout.session.completed).Mensajería de Eventos: MassTransit + RabbitMQ (Bus de eventos asíncrono y gestión de colas).Worker en Segundo Plano: Servicio Worker en .NET 9 (ServiceNowWorker) para el consumo de eventos de alerta.Contenedores: Dockerfiles multi-etapa orquestados mediante docker-compose.Infraestructura como Código (IaC):Terraform: Aprovisionamiento de Grupos de Recursos, Redes Virtuales y Clúster AKS en Azure.Ansible: Automatización de configuraciones y despliegue de manifiestos en Kubernetes.Python: Ejecutor de la pipeline automatizada (deploy_azure.py).Sistema de Diseño UI/UX: Feira GallegaEl sistema de diseño aplica la estética del comercio tradicional gallego respetando los estándares de accesibilidad WCAG 2.1:Token de DiseñoColor HexUso en la InterfazFondo Principal#F9F8F6Fondo orgánico tono Lino / Hueso cálidoMarca Principal#0A2540Azul Mariño Profundo para títulos, headers y footerAcento Tradicional#C85A32Terracota / Tello para etiquetas "Feira" y avisosAcción Principal (CTA)#2D6A4FVerde Orgánico para botones de compra y estados activosGris Secundario#6B7280Textos secundarios, metadatos y nombres de feirantesAnillo de Foco#80BFFFOutline de accesibilidad de 3px para navegación por tecladoEstructura del RepositorioPlaintext.
├── docker-compose.yml          # Orquestación de contenedores para entorno local
├── src/
│   ├── backend/                # API Minimal ASP.NET Core 9.0
│   │   ├── Dockerfile
│   │   ├── EcommerceApi.csproj
│   │   ├── appsettings.json    # Claves de Stripe y configuración JWT
│   │   └── Program.cs          # Minimal APIs, SDK Stripe y configuración MassTransit
│   ├── frontend/               # SPA Angular 18
│   │   ├── Dockerfile
│   │   ├── nginx.conf          # Configuración del Proxy Inverso NGINX
│   │   ├── src/
│   │   │   ├── main.ts         # Componente principal, signals, pagos Stripe y llamadas HTTP
│   │   │   ├── styles.css      # Tokens de diseño y reglas CSS globales
│   │   │   └── app/
│   │   │       └── components/language-switcher/
│   │   └── package.json
│   └── workers/                # Worker ServiceNow en segundo plano
│       ├── Dockerfile
│       ├── Program.cs
│       └── ServiceNowWorker.csproj
└── infra/                      # Infraestructura como Código
    ├── terraform/              # Aprovisionamiento de clúster Azure AKS
    ├── ansible/                # Playbooks para Kubernetes
    └── scripts/                # Scripts de automatización
Guía de Ejecución LocalRequisitos PreviosDocker Engine 24+ y Docker Compose v2+GitStripe CLI (para la prueba local de webhooks)Pasos para la EjecuciónClonar el repositorio:Bashgit clone [https://github.com/Snow20/Mercadillo-Ecommerce.git](https://github.com/Snow20/Mercadillo-Ecommerce.git)
cd Mercadillo-Ecommerce
Configurar las claves de Stripe en variables de entorno en docker-compose.yml o en appsettings.json:JSON"Stripe": {
  "SecretKey": "sk_test_...",
  "PublishableKey": "pk_test_...",
  "WebhookSecret": "whsec_..."
}
Compilar y levantar los servicios:Bashdocker-compose up --build -d
Verificar los servicios activos:Aplicación E-Commerce Frontend: http://localhostDocumentación Swagger API Backend: http://localhost:5000/swaggerPanel de Administración RabbitMQ: http://localhost:15672 (Usuario: guest | Clave: guest)Detener el entorno local:Bashdocker-compose down
Integración de Stripe Checkout y WebhooksAbre el navegador e ingresa a http://localhost.Pulsa en 💳 Pagar con Stripe en cualquiera de las tarjetas del catálogo (ej. Polbo á Feira o Queixo Arzúa-Ulloa).El cliente Angular solicita la sesión al backend en .NET 9 (/api/payment/create-checkout-session) y redirige al usuario a la pasarela segura de Stripe Checkout.Datos de Tarjeta de Prueba:Número de Tarjeta: 4242 4242 4242 4242Expiración: Cualquier fecha futura (ej. 12/28)CVC: 123Reenvío Local de Webhooks (Stripe CLI):Bashstripe listen --events checkout.session.completed --forward-to localhost/api/payment/webhook
Disparar Evento de Prueba Manual:Bashstripe trigger checkout.session.completed
Resultado esperado: Respuesta HTTP 200 OK emitida por la API de .NET 9 registrando en logs: [Stripe Webhook OK]: Pago confirmado para la sesión cs_test_....Endpoints de la API e Integración1. Endpoint del CatálogoGET /api/catalogDescripción: Devuelve la lista completa de los 10 productos artesanales gallegos.Respuesta de Ejemplo:JSON[
  { "id": 1, "name": "Polbo á Feira (Ración)", "price": 18.50 },
  { "id": 2, "name": "Queixo Arzúa-Ulloa DOP", "price": 9.20 }
]
2. Endpoint del Carrito de ComprasPOST /api/cartDescripción: Recibe las órdenes de compra enviadas desde el frontend.Carga Útil (Payload):JSON{
  "productId": 1,
  "productName": "Polbo á Feira (Ración)",
  "price": 18.50
}
3. Endpoint de Sesión Stripe CheckoutPOST /api/payment/create-checkout-sessionDescripción: Genera la URL oficial de la pasarela de pago segura de Stripe.Carga Útil (Payload):JSON{
  "productId": 1,
  "productName": "Polbo á Feira (Ración)",
  "price": 18.50
}
Respuesta de Ejemplo:JSON{
  "sessionId": "cs_test_a1b2c3...",
  "url": "[https://checkout.stripe.com/c/pay/cs_test_a1b2c3](https://checkout.stripe.com/c/pay/cs_test_a1b2c3)..."
}
4. Endpoint Receptor de Webhooks de StripePOST /api/payment/webhookDescripción: Consume los eventos criptográficos de Stripe, verifica la firma (Stripe-Signature) y procesa la confirmación de la compra.5. Endpoint de Alerta e Incidencias (ServiceNow)POST /api/incidentsDescripción: Publica un evento IncidentAlertEvent en RabbitMQ. El servicio servicenow-worker consume el mensaje y da de alta la incidencia en ServiceNow.Carga Útil (Payload):JSON{
  "title": "Incidencia en producto: Polbo á Feira",
  "description": "Alerta generada desde la web para el producto ID 1",
  "severity": "HIGH"
}
Infraestructura y Aprovisionamiento CloudPara desplegar la infraestructura en la nube sobre Azure AKS utilizando IaC:Iniciar sesión en Azure CLI:Bashaz login
Ejecutar el script automatizado:Bashpython3 infra/scripts/deploy_azure.py
License & CreditsDeveloped by Carlos Nieves / Fenrirsoft © 2026. All rights reserved / Todos los derechos reservados.