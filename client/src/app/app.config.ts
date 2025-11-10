import { ApplicationConfig, DEFAULT_CURRENCY_CODE, provideBrowserGlobalErrorListeners, provideZoneChangeDetection, APP_INITIALIZER, inject } from '@angular/core';
import { provideRouter } from '@angular/router';

 import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { Account } from './core/services/account';
import { authInterceptor } from './core/Interceptors/auth-interceptor';

function initializeApp() {
  const accountService = inject(Account);
  return () => {
    return accountService.getCurrentUser().toPromise().catch(() => {
      // Ignore errors during initialization
      return null;
    });
  };
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    {provide: DEFAULT_CURRENCY_CODE, useValue: 'EUR'},
    {provide: APP_INITIALIZER, useFactory: initializeApp, multi: true}
  ]
};
