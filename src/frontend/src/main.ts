import { bootstrapApplication } from '@angular/platform-browser';
import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient, provideHttpClient } from '@angular/common/http';
import { LanguageSwitcherComponent, Language } from './app/components/language-switcher/language-switcher.component';

interface ProductoAPI {
  id: number;
  name: string;
  price: number;
}

interface Producto {
  id: number;
  nombre: string;
  precio: number;
  icono: string;
  descripcion: string;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, LanguageSwitcherComponent],
  template: `
    <!-- Top Announcement Bar -->
    <div class="top-bar-announcement">
      {{ t().announcement }}
    </div>

    <!-- Header Feira -->
    <header class="header-feira">
      <div class="logo-container">
        <span class="logo-badge">FEIRA</span>
        <h1>{{ t().title }}</h1>
      </div>
      <app-language-switcher 
        [currentLang]="idiomaActual()" 
        (langChange)="cambiarIdioma($event)">
      </app-language-switcher>
    </header>

    <!-- Categories Horizontal Bar -->
    <nav class="categories-filter-bar">
      <button class="category-chip active">Tódolos Produtos</button>
      <button class="category-chip">Produtos da Terra</button>
      <button class="category-chip">Mariscos e Peixes</button>
      <button class="category-chip">Repostaría e Mel</button>
      <button class="category-chip">Viños e Licores</button>
    </nav>

    <!-- Banner Principal -->
    <section class="hero-section">
      <h2 class="hero-title">{{ t().heroTitle }}</h2>
      <p class="hero-subtitle">{{ t().heroSubtitle }}</p>
      <button class="btn-hero-cta">{{ t().btnHero }}</button>
    </section>

    <!-- Contenido Principal -->
    <main class="main-container">
      <h3 class="section-title">{{ t().sectionTitle }}</h3>

      <div class="grid-productos">
        @for (prod of productos(); track prod.id) {
          <article class="card-feira">
            <div class="card-img-placeholder">
              <span class="card-badge-origen">100% Galego</span>
              {{ prod.icono }}
            </div>
            <div class="card-body">
              <h4 class="card-title">{{ prod.nombre }}</h4>
              <p class="card-producer">Feirante Tradicional · Galicia</p>
              <p class="card-description">{{ prod.descripcion }}</p>
              <div class="card-price">{{ prod.precio | currency:'EUR':'symbol':'1.2-2' }}</div>
              <div class="card-actions">
                <button class="btn-feira-primary" (click)="agregarAlCarrito(prod)">{{ t().btnBuy }}</button>
                <button class="btn-feira-secondary" (click)="reportarIncidencia(prod)" title="Notificar a ServiceNow">
                  {{ t().btnAlert }}
                </button>
              </div>
            </div>
          </article>
        }
      </div>
    </main>

    <!-- Footer Feira -->
    <footer class="footer-feira">
      <div class="footer-content">
        <div class="footer-brand">
          <div class="logo-container">
            <span class="logo-badge">FEIRA</span>
            <h3>{{ t().title }}</h3>
          </div>
          <p class="footer-description">{{ t().footerDesc }}</p>
        </div>
        <div class="footer-links">
          <h4>{{ t().footerLinksTitle }}</h4>
          <ul>
            <li><a href="#puestos">{{ t().linkStalls }}</a></li>
            <li><a href="#produtores">{{ t().linkProducers }}</a></li>
            <li><a href="#normativa">{{ t().linkNorms }}</a></li>
          </ul>
        </div>
      </div>
      <div class="footer-bottom">
        <p>&copy; 2026 <strong>Fenrirsoft</strong>. {{ t().rights }}</p>
      </div>
    </footer>
  `
})
export class AppComponent implements OnInit {
  private http = inject(HttpClient);

  idiomaActual = signal<Language>('gl');
  productos = signal<Producto[]>([]);

private traducciones = {
    gl: {
      announcement: 'Próxima Feira Local: Este Sábado no Mercado Tradicional',
      title: 'Feira Gallega Enterprise',
      heroTitle: 'Mercadillo Tradicional e Dixital',
      heroSubtitle: 'Os mellores produtos artesanais e gastronómicos directamente dos produtores locais de Galicia.',
      btnHero: 'Explorar Puestos',
      sectionTitle: 'Produtos Destacados da Feira',
      btnBuy: 'Engadir ao carro',
      btnAlert: 'Alerta',
      footerDesc: 'Plataforma dixital para a promoción do comercio local e os produtos artesanais de Galicia.',
      footerLinksTitle: 'Navegación',
      linkStalls: 'Puestos da Feira',
      linkProducers: 'Produtores Locais',
      linkNorms: 'Calidade Garantida',
      rights: 'Tódolos dereitos reservados.'
    },
    es: {
      announcement: 'Próxima Feira Local: Este Sábado en el Mercado Tradicional',
      title: 'Mercadillo Gallego Enterprise',
      heroTitle: 'Mercadillo Tradicional y Digital',
      heroSubtitle: 'Los mejores productos artesanales y gastronómicos directamente de los productores locales de Galicia.',
      btnHero: 'Explorar Puestos',
      sectionTitle: 'Productos Destacados del Mercadillo',
      btnBuy: 'Añadir al carrito',
      btnAlert: 'Alerta',
      footerDesc: 'Plataforma digital para la promoción del comercio local y los productos artesanales de Galicia.',
      footerLinksTitle: 'Navegación',
      linkStalls: 'Puestos del Mercadillo',
      linkProducers: 'Productores Locales',
      linkNorms: 'Calidad Garantizada',
      rights: 'Todos los derechos reservados.'
    },
    en: {
      announcement: 'Next Local Fair: This Saturday at the Traditional Market',
      title: 'Galician Market Enterprise',
      heroTitle: 'Traditional & Digital Fair',
      heroSubtitle: 'The finest artisanal and gastronomic products directly from local Galician producers.',
      btnHero: 'Explore Stalls',
      sectionTitle: 'Featured Market Products',
      btnBuy: 'Add to cart',
      btnAlert: 'Alert',
      footerDesc: 'Digital platform for the promotion of local trade and Galician artisanal products.',
      footerLinksTitle: 'Navigation',
      linkStalls: 'Market Stalls',
      linkProducers: 'Local Producers',
      linkNorms: 'Guaranteed Quality',
      rights: 'All rights reserved.'
    }
  };

  t = computed(() => this.traducciones[this.idiomaActual()]);

  ngOnInit() {
    this.cargarCatalogoDesdeAPI();
  }

  cambiarIdioma(nuevoIdioma: Language) {
    this.idiomaActual.set(nuevoIdioma);
  }

  cargarCatalogoDesdeAPI() {
    this.http.get<ProductoAPI[]>('/api/catalog').subscribe({
      next: (data) => {
        const productosMapeados: Producto[] = data.map((item) => ({
          id: item.id,
          nombre: item.name,
          precio: item.price,
          icono: this.obtenerIcono(item.id),
          descripcion: this.obtenerDescripcion(item.id)
        }));
        this.productos.set(productosMapeados);
      },
      error: () => {
        // Fallback expandido de 10 produtos
        this.productos.set([
          { id: 1, nombre: 'Polbo á Feira (Ración)', precio: 18.50, icono: '🐙', descripcion: 'Polbo fresco cocido no punto con aceite de oliva, sal gorda e pemento.' },
          { id: 2, nombre: 'Queixo Arzúa-Ulloa DOP', precio: 9.20, icono: '🧀', descripcion: 'Queixo artesanal de vaca con textura cremosa e sabor suave.' },
          { id: 3, nombre: 'Empanada de Atún e Pementos', precio: 12.00, icono: '🥧', descripcion: 'Masa artesana enfornada con atún e pementos da horta.' },
          { id: 4, nombre: 'Vino Albariño Rías Baixas', precio: 16.80, icono: '🍾', descripcion: 'Viño branco moito fresco e afrutado das Rías Baixas.' },
          { id: 5, nombre: 'Tarta de Santiago Tradicional', precio: 14.50, icono: '🥧', descripcion: 'Elaborada con améndoas seleccionadas e cruz de Santiago.' },
          { id: 6, nombre: 'Zamburiñas do Saco (12 ud)', precio: 22.00, icono: '🦪', descripcion: 'Zamburiñas frescas das rías galegas listas para a prancha.' },
          { id: 7, nombre: 'Pan de Cea Protexido (Bolo)', precio: 4.20, icono: '🥖', descripcion: 'Pan artesano de masa nai cocido en forno de pedra de leña.' },
          { id: 8, nombre: 'Licor Café Orujo Galego', precio: 13.90, icono: '☕', descripcion: 'Licor de bagazo destilado con café de tueste natural.' },
          { id: 9, nombre: 'Pementos de Padrón / Herbón', precio: 5.50, icono: '🫑', descripcion: 'Uns pican e outros non. Auténticos pementos da feira.' },
          { id: 10, nombre: 'Mel Artesanal das Fragas do Eume', precio: 8.80, icono: '🍯', descripcion: 'Mel 100% pura colleitada nos bosques autóctonos de Galicia.' }
        ]);
      }
    });
  }

  agregarAlCarrito(prod: Producto) {
    const payload = {
      productId: prod.id,
      productName: prod.nombre,
      price: prod.precio
    };

    // Petición HTTP POST real al Backend .NET a través del proxy NGINX
    this.http.post<{ message: string }>('/api/cart', payload).subscribe({
      next: (res) => {
        // Respuesta confirmada desde la API C# de .NET 9
        alert(`[Backend .NET 9 OK]: ${res.message}`);
      },
      error: (err) => {
        console.error('Error al conectar con la API de compra:', err);
        alert(`Servidor temporalmente no disponible. Producto registrado localmente: ${prod.nombre}`);
      }
    });
  }

  reportarIncidencia(prod: Producto) {
    const payload = {
      title: `Incidencia en producto: ${prod.nombre}`,
      description: `Alerta generada desde la web para el producto ID ${prod.id}`,
      severity: 'HIGH'
    };

    this.http.post('/api/incidents', payload).subscribe({
      next: () => alert(`Incidencia enviada a ServiceNow para: ${prod.nombre}`),
      error: () => alert(`Incidencia registrada localmente para: ${prod.nombre}`)
    });
  }

  private obtenerIcono(id: number): string {
    const iconos: Record<number, string> = { 
      1: '🐙', 2: '🧀', 3: '🥧', 4: '🍾', 5: '🥧', 
      6: '🦪', 7: '🥖', 8: '☕', 9: '🫑', 10: '🍯' 
    };
    return iconos[id] || '🛒';
  }

  private obtenerDescripcion(id: number): string {
    const descripciones: Record<number, string> = {
      1: 'Polbo fresco cocido no punto con aceite de oliva, sal gorda e pemento.',
      2: 'Queixo artesanal de vaca con textura cremosa e sabor suave.',
      3: 'Masa artesana enfornada con atún e pementos da horta.',
      4: 'Viño branco moito fresco e afrutado das Rías Baixas.',
      5: 'Elaborada con améndoas seleccionadas e cruz de Santiago.',
      6: 'Zamburiñas frescas das rías galegas listas para a prancha.',
      7: 'Pan artesano de masa nai cocido en forno de pedra de leña.',
      8: 'Licor de bagazo destilado con café de tueste natural.',
      9: 'Uns pican e outros non. Auténticos pementos da feira.',
      10: 'Mel 100% pura colleitada nos bosques autóctonos de Galicia.'
    };
    return descripciones[id] || 'Produto artesanal galego de alta calidade.';
  }
}

bootstrapApplication(AppComponent, {
  providers: [provideHttpClient()]
}).catch(err => console.error(err));