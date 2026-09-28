# Feira Gallega Enterprise - Modular E-Commerce Architecture

A high-performance, enterprise-grade e-commerce platform built with a modern decoupled architecture: Angular 18 frontend with a custom Galician Market UI/UX design system, .NET 9 Minimal APIs backend with Multi-Gateway Payment Architecture (Factory Pattern), asynchronous event-driven messaging with MassTransit and RabbitMQ, a dedicated ServiceNow Incident Worker, and Infrastructure as Code (IaC) for Azure AKS deployment.

---

## Table of Contents
1. [English Version](#english-version)
   - [Project Overview](#project-overview)
   - [Architectural Blueprint](#architectural-blueprint)
   - [Tech Stack](#tech-stack)
   - [UI/UX Design System: Feira Gallega](#uiux-design-system-feira-gallega)
   - [Repository Structure](#repository-structure)
   - [Getting Started (Local Execution)](#getting-started-local-execution)
   - [Multi-Gateway Architecture & Webhooks](#multi-gateway-architecture--webhooks)
   - [API Endpoints & Integration](#api-endpoints--integration)
   - [Infrastructure & Cloud Provisioning](#infrastructure--cloud-provisioning)
2. [Versión en Español](#versión-en-español)
   - [Visión General del Proyecto](#visión-general-del-proyecto)
   - [Arquitectura del Sistema](#arquitectura-del-sistema)
   - [Tecnologías Utilizadas](#tecnologías-utilizadas)
   - [Sistema de Diseño UI/UX: Feira Gallega](#sistema-de-diseño-uiux-feira-gallega)
   - [Estructura del Repositorio](#estructura-del-repositorio)
   - [Guía de Ejecución Local](#guía-de-ejecución-local)
   - [Arquitectura Multi-Pasarela y Webhooks](#arquitectura-multi-pasarela-y-webhooks)
   - [Endpoints de la API e Integración](#endpoints-de-la-api-e-integración)
   - [Infraestructura y Aprovisionamiento Cloud](#infraestructura-y-aprovisionamiento-cloud)
3. [License & Credits](#license--credits)

---

## English Version

### Project Overview
**Feira Gallega Enterprise** is a production-ready blueprint for decoupled e-commerce solutions. It features a modern SPA frontend built with Angular 18 that reflects the traditional yet modern Galician fair aesthetic, backed by a microservices-capable .NET 9 Web API. Payments are decoupled via the **Factory / Strategy Pattern**, enabling multi-gateway checkouts across **Stripe, PayPal, Abanca (Redsys), Banco Santander, and Wise**. Card payments are processed via **Stripe Checkout API** and verified asynchronously through signed webhooks. Critical system alerts and support requests are handled asynchronously through RabbitMQ and consumed by an automated worker logging incidents into ServiceNow via REST APIs.

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
             [ PaymentGatewayFactory ]    [ RabbitMQ Broker ]
            ┌──────────┬──────────┐               │
            │          │          │               │ (Consume Queue)
         [Stripe]  [PayPal]   [Abanca/...]        ▼
            │                               [ ServiceNow Worker ]
  (Signed Webhook)                                │
            │                             (Basic Auth REST)
            ▼                                     │
   [ Webhook Endpoint ]                           ▼
                                      [ ServiceNow Instance ]

---

### Tech Stack

* **Frontend:** Angular 18 (Standalone Components, Signals, Reactive Forms, i18n support for GL, ES, EN, `@stripe/stripe-js`).
* **Reverse Proxy:** NGINX Alpine (Route mapping, CORS elimination, static asset hosting).
* **Backend API:** ASP.NET Core 9.0 (Minimal APIs, Factory Pattern, Stripe.net SDK, JWT Bearer Authentication, Swagger / OpenAPI).
* **Payment Gateways:** Decoupled Architecture (`IPaymentGatewayService`) supporting Stripe Checkout, PayPal, Abanca (Redsys), Santander, and Wise.
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
│   │   └── src/API/
│   │       ├── Program.cs      # Minimal APIs & Dependency Injection setup
│   │       └── Services/       # PaymentGatewayFactory & IPaymentGatewayService classes
│   ├── frontend/               # Angular 18 SPA
│   │   ├── Dockerfile
│   │   ├── nginx.conf          # Reverse Proxy routing configuration
│   │   ├── src/
│   │   │   ├── main.ts         # Main component, signals, payment selector & HTTP requests
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
Getting Started (Local Execution)PrerequisitesDocker Engine 24+ & Docker Compose v2+   Git   Stripe CLI (for local webhook testing)   Execution StepsClone the repository:   Bashgit clone [https://github.com/Snow20/Mercadillo-Ecommerce.git](https://github.com/Snow20/Mercadillo-Ecommerce.git)
cd Mercadillo-Ecommerce
Configure Stripe API keys in docker-compose.yml or appsettings.json:   JSON"Stripe": {
  "SecretKey": "sk_test_...",
  "PublishableKey": "pk_test_...",
  "WebhookSecret": "whsec_..."
}
Build and start the container stack:   Bashdocker-compose up --build -d
Verify running services:   Frontend & E-Commerce Application: http://localhost   Backend API Swagger Documentation: http://localhost:5000/swagger   RabbitMQ Management Dashboard: http://localhost:15672 (User: guest | Pass: guest)   Stop local environment:   Bashdocker-compose down
Multi-Gateway Architecture & WebhooksAccess the web interface at http://localhost.   Select your preferred payment gateway from the dropdown on any product card (Stripe, PayPal, Abanca, Santander, or Wise).Click Pagar. The Angular client submits the payload to /api/payment/checkout, where PaymentGatewayFactory instantiates the selected provider implementation.Local Webhook Event Forwarding (Stripe CLI):   Bashstripe listen --events checkout.session.completed --forward-to localhost/api/payment/webhook
Trigger Manual Test Event:   Bashstripe trigger checkout.session.completed
Expected Result: HTTP 200 OK returned by .NET 9 API with log output: [Stripe Webhook OK]: Pago confirmado para la sesión cs_test_....   API Endpoints & Integration1. Catalog EndpointGET /api/catalog   Description: Returns the complete list of 10 Galician artisanal products.   Sample Response:   JSON[
  { "id": 1, "name": "Polbo á Feira (Ración)", "price": 18.50 },
  { "id": 2, "name": "Queixo Arzúa-Ulloa DOP", "price": 9.20 }
]
2. Unified Checkout Endpoint (Factory Pattern)POST /api/payment/checkoutDescription: Generates checkout sessions for the requested payment provider (Stripe, PayPal, Abanca, Santander, or Wise).Payload:JSON{
  "productId": 1,
  "productName": "Polbo á Feira (Ración)",
  "price": 18.50,
  "provider": "Stripe"
}
Sample Response:JSON{
  "sessionId": "cs_test_a1b2c3...",
  "url": "[https://checkout.stripe.com/c/pay/cs_test_a1b2c3](https://checkout.stripe.com/c/pay/cs_test_a1b2c3)..."
}
3. Legacy Stripe Checkout EndpointPOST /api/payment/create-checkout-session   Description: Legacy route preserved for backward compatibility with Stripe Checkout sessions.   4. Stripe Webhook Listener EndpointPOST /api/payment/webhook   Description: Consumes cryptographic Stripe webhooks, validates signatures (Stripe-Signature), and processes transaction completions.   5. Incident Alert Endpoint (ServiceNow Integration)POST /api/incidents   Description: Publishes an IncidentAlertEvent message to RabbitMQ. The servicenow-worker consumes the queue and creates an incident in ServiceNow.   Infrastructure & Cloud ProvisioningTo deploy the platform to Azure AKS using IaC:   Login to Azure CLI:   Bashaz login
Run automated pipeline:   Bashpython3 infra/scripts/deploy_azure.py
Versión en EspañolVisión General del ProyectoFeira Gallega Enterprise es una arquitectura de referencia para la construcción de plataformas e-commerce desacopladas. Cuenta con un frontend SPA en Angular 18 diseñado bajo la estética visual de una "Feira Gallega Moderna", respaldado por una API REST en .NET 9. La capa de pago está desacoplada mediante el Patrón Factory / Strategy, permitiendo procesar compras a través de Stripe, PayPal, Abanca (Redsys), Banco Santander y Wise. Los pagos con tarjeta se procesan mediante la API de Stripe Checkout y la ingesta asíncrona de Webhooks firmados. El sistema incluye procesamiento de eventos con RabbitMQ para notificar incidencias técnicas en tiempo real a ServiceNow mediante un servicio Worker independiente.   Arquitectura del Sistema         [ Cliente / Navegador Web ]
                    │
                    │ (HTTP / Puerto 80)
                    ▼
       [ NGINX Proxy Inverso ]
       ┌────────────┴────────────┐
       │                         │
(Archivos Estáticos)      (Proxy API /api/*)
       │                         │
       ▼                         ▼
[ Frontend Angular 18 ]    [ API Backend .NET 9 ]│         │(Crear Checkout)  │         │ (Publica Evento Incidencia)▼         ▼[ PaymentGatewayFactory ]    [ Broker RabbitMQ ]┌──────────┬──────────┐               ││          │          │               │ (Consume Cola)[Stripe]  [PayPal]   [Abanca/...]        ▼│                               [ Worker ServiceNow ](Webhook Firmado)                           ││                             (Basic Auth REST)▼                                     │[ Endpoint Webhook ]                           ▼[ Instancia ServiceNow ]Tecnologías UtilizadasFrontend: Angular 18 (Componentes Standalone, Signals, Formularios Reactivos, Selector multilingüe: GL, ES, EN, @stripe/stripe-js).   Reverse Proxy: NGINX Alpine (Enrutamiento de red, eliminación de CORS, entrega de estáticos).   Backend API: ASP.NET Core 9.0 (Minimal APIs, Patrón Factory, SDK Stripe.net, Autenticación JWT Bearer, Documentación Swagger / OpenAPI).   Pasarelas de Pago: Arquitectura desacoplada (IPaymentGatewayService) con soporte para Stripe, PayPal, Abanca (Redsys), Santander y Wise.   Mensajería de Eventos: MassTransit + RabbitMQ (Bus de eventos asíncrono y gestión de colas).   Worker en Segundo Plano: Servicio Worker en .NET 9 (ServiceNowWorker) para el consumo de eventos de alerta.   Contenedores: Dockerfiles multi-etapa orquestados mediante docker-compose.   Infraestructura como Código (IaC):   Terraform: Aprovisionamiento de Grupos de Recursos, Redes Virtuales y Clúster AKS en Azure.   Ansible: Automatización de configuraciones y despliegue de manifiestos en Kubernetes.   Python: Ejecutor de la pipeline automatizada (deploy_azure.py).   Sistema de Diseño UI/UX: Feira GallegaEl sistema de diseño aplica la estética del comercio tradicional gallego respetando los estándares de accesibilidad WCAG 2.1:   Token de DiseñoColor HexUso en la InterfazFondo Principal#F9F8F6Fondo orgánico tono Lino / Hueso cálido   Marca Principal#0A2540Azul Mariño Profundo para títulos, headers y footer   Acento Tradicional#C85A32Terracota / Tello para etiquetas "Feira" y avisos   Acción Principal (CTA)#2D6A4FVerde Orgánico para botones de compra y estados activos   Gris Secundario#6B7280Textos secundarios, metadatos y nombres de feirantes   Anillo de Foco#80BFFFOutline de accesibilidad de 3px para navegación por teclado   Estructura del RepositorioPlaintext.
├── docker-compose.yml          # Orquestación de contenedores para entorno local
├── src/
│   ├── backend/                # API Minimal ASP.NET Core 9.0
│   │   ├── Dockerfile
│   │   ├── EcommerceApi.csproj
│   │   ├── appsettings.json    # Claves de Stripe y configuración JWT
│   │   └── src/API/
│   │       ├── Program.cs      # Minimal APIs y registro de Inyección de Dependencias
│   │       └── Services/       # PaymentGatewayFactory y servicios de pasarelas
│   ├── frontend/               # SPA Angular 18
│   │   ├── Dockerfile
│   │   ├── nginx.conf          # Configuración del Proxy Inverso NGINX
│   │   ├── src/
│   │   │   ├── main.ts         # Componente principal, signals, pagos multi-pasarela y HTTP
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
Guía de Ejecución LocalRequisitos PreviosDocker Engine 24+ y Docker Compose v2+   Git   Stripe CLI (para la prueba local de webhooks)   Pasos para la EjecuciónClonar el repositorio:   Bashgit clone [https://github.com/Snow20/Mercadillo-Ecommerce.git](https://github.com/Snow20/Mercadillo-Ecommerce.git)
cd Mercadillo-Ecommerce
Configurar las claves de Stripe en docker-compose.yml o appsettings.json:   JSON"Stripe": {
  "SecretKey": "sk_test_...",
  "PublishableKey": "pk_test_...",
  "WebhookSecret": "whsec_..."
}
Compilar y levantar los servicios:   Bashdocker-compose up --build -d
Verificar los servicios activos:[cite: 8]Aplicación E-Commerce Frontend: http://localhost[cite: 8]Documentación Swagger API Backend: http://localhost:5000/swagger[cite: 8]Panel de Administración RabbitMQ: http://localhost:15672 (Usuario: guest | Clave: guest)[cite: 8]Detener el entorno local:[cite: 8]Bashdocker-compose down
Arquitectura Multi-Pasarela y WebhooksIngresa en el navegador a http://localhost[cite: 8].Selecciona el proveedor de pago deseado en el selector desplegable de cada tarjeta (Stripe, PayPal, Abanca, Santander o Wise).Haz clic en Pagar. El cliente Angular enviará la solicitud a /api/payment/checkout, donde PaymentGatewayFactory resolverá la pasarela correspondiente.Reenvío Local de Webhooks (Stripe CLI):[cite: 8]Bashstripe listen --events checkout.session.completed --forward-to localhost/api/payment/webhook
Disparar Evento de Prueba Manual:[cite: 8]Bashstripe trigger checkout.session.completed
Resultado esperado: Respuesta HTTP 200 OK emitida por la API registrando en logs: [Stripe Webhook OK]: Pago confirmado para la sesión cs_test_...[cite: 8].Endpoints de la API e Integración1. Endpoint del CatálogoGET /api/catalog[cite: 8]Descripción: Devuelve la lista completa de los 10 productos artesanales gallegos[cite: 8].2. Endpoint Unificado de Pago (Factory Pattern)POST /api/payment/checkoutDescripción: Procesa el inicio de compra delegando en el proveedor seleccionado (Stripe, PayPal, Abanca, Santander o Wise).Carga Útil (Payload):JSON{
  "productId": 1,
  "productName": "Polbo á Feira (Ración)",
  "price": 18.50,
  "provider": "Stripe"
}
3. Endpoint Legado de Stripe CheckoutPOST /api/payment/create-checkout-session[cite: 8]Descripción: Endpoint mantenido por compatibilidad para generar sesiones de Stripe Checkout[cite: 8].4. Endpoint Receptor de Webhooks de StripePOST /api/payment/webhook[cite: 8]Descripción: Consume los eventos criptográficos de Stripe, verifica la firma (Stripe-Signature) y procesa la confirmación de compra[cite: 8].5. Endpoint de Alerta e Incidencias (ServiceNow)POST /api/incidents[cite: 8]Descripción: Publica un evento IncidentAlertEvent en RabbitMQ[cite: 8]. El servicio servicenow-worker consume el mensaje y da de alta la incidencia en ServiceNow[cite: 8].Infraestructura y Aprovisionamiento CloudPara desplegar la infraestructura en la nube sobre Azure AKS utilizando IaC:[cite: 8]Iniciar sesión en Azure CLI:[cite: 8]Bashaz login
Ejecutar el script automatizado:[cite: 8]Bashpython3 infra/scripts/deploy_azure.py
License & CreditsDeveloped by Carlos Nieves / Fenrirsoft © 2026. All rights reserved / Todos los derechos reservados.[cite: 8]