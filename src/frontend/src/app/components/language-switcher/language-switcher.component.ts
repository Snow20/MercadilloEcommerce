import { Component, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

export type Language = 'gl' | 'es' | 'en';

@Component({
  selector: 'app-language-switcher',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="feira-lang-switch">
      <span class="label-feira">Linguaxe:</span>
      <div class="toggle-container">
        <!-- Galicia / Galego -->
        <button 
          class="flag-switch" 
          [class.active]="currentLang() === 'gl'"
          (click)="setLanguage('gl')"
          title="Galego normativo">
          <svg class="flag-icon" viewBox="0 0 600 400">
            <rect width="600" height="400" fill="#ffffff"/>
            <path d="M 0,0 L 600,400 L 600,270 L 195,0 Z" fill="#008cb4"/>
            <path d="M 0,130 L 405,400 L 0,400 Z" fill="#008cb4"/>
          </svg>
          <span class="lang-code">GL</span>
        </button>

        <!-- España / Español -->
        <button 
          class="flag-switch" 
          [class.active]="currentLang() === 'es'"
          (click)="setLanguage('es')"
          title="Español">
          <svg class="flag-icon" viewBox="0 0 750 500">
            <rect width="750" height="500" fill="#c60b1e"/>
            <rect y="125" width="750" height="250" fill="#ffc400"/>
          </svg>
          <span class="lang-code">ES</span>
        </button>

        <!-- UK / English -->
        <button 
          class="flag-switch" 
          [class.active]="currentLang() === 'en'"
          (click)="setLanguage('en')"
          title="English">
          <svg class="flag-icon" viewBox="0 0 600 300">
            <clipPath id="s"><path d="M0,0 v300 h600 v-300 z"/></clipPath>
            <path d="M0,0 L600,300 M0,300 L600,0" stroke="#fff" stroke-width="60"/>
            <path d="M0,0 L600,300 M0,300 L600,0" stroke="#012169" stroke-width="40"/>
            <path d="M300,0 V300 M0,150 H600" stroke="#fff" stroke-width="100"/>
            <path d="M300,0 V300 M0,150 H600" stroke="#C8102E" stroke-width="60"/>
          </svg>
          <span class="lang-code">EN</span>
        </button>
      </div>
    </div>
  `,
  styles: [`
    .feira-lang-switch {
      display: flex;
      align-items: center;
      gap: 10px;
      background: rgba(92, 44, 6, 0.85);
      padding: 6px 14px;
      border-radius: 25px;
      border: 2px solid #d4af37;
      box-shadow: 0 4px 10px rgba(0,0,0,0.3);
    }
    .label-feira {
      color: #fdfbf7;
      font-weight: 700;
      font-size: 0.85rem;
      letter-spacing: 0.5px;
    }
    .toggle-container {
      display: flex;
      background: #2d1401;
      border-radius: 18px;
      padding: 3px;
      gap: 4px;
    }
    .flag-switch {
      display: flex;
      align-items: center;
      gap: 5px;
      border: none;
      background: transparent;
      padding: 4px 10px;
      border-radius: 14px;
      cursor: pointer;
      opacity: 0.55;
      transition: all 0.25s ease-in-out;
    }
    .flag-switch.active {
      opacity: 1;
      background: #ffffff;
      box-shadow: 0 2px 6px rgba(0,0,0,0.35);
      transform: scale(1.04);
    }
    .flag-icon {
      width: 20px;
      height: 14px;
      border-radius: 2px;
    }
    .lang-code {
      font-size: 0.75rem;
      font-weight: 800;
      color: #2d1401;
    }
  `]
})
export class LanguageSwitcherComponent {
  currentLang = input<Language>('gl');
  langChange = output<Language>();

  setLanguage(lang: Language) {
    this.langChange.emit(lang);
  }
}