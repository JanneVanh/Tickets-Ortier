import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot } from '@angular/router';
import { Account } from '../services/account';
import { map, of } from 'rxjs';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state) => {
  const accountService = inject(Account);
  const router = inject(Router);
  const requiredRoles = route.data['roles'] as string[] || [];

  if (accountService.currentUser()) {
    const user = accountService.currentUser();
    if (user && hasRequiredRole(user.roles, requiredRoles)) {
      return of(true);
    } else {
      router.navigate(['/']); // Redirect to home if not authorized
      return of(false);
    }
  } else {
    return accountService.getAuthState().pipe(
      map(auth => {
        if (auth.isAuthenticated) {
          if (auth.user) {
            // Update current user with received data
            accountService.currentUser.set(auth.user);
            if (hasRequiredRole(auth.user.roles, requiredRoles)) {
              return true;
            } else {
              router.navigate(['/']); // Redirect to home if not authorized
              return false;
            }
          } else {
            // If authenticated but no user data in response, get user data
            accountService.getCurrentUser().subscribe(user => {
              if (user && hasRequiredRole(user.roles, requiredRoles)) {
                router.navigate([state.url]); // Navigate to original URL
              } else {
                router.navigate(['/']); // Redirect to home if not authorized
              }
            });
            return false; // Block initially while getting user data
          }
        } else {
          router.navigate(['/account/login'], { queryParams: { returnUrl: state.url } });
          return false;
        }
      })
    );
  }
};

function hasRequiredRole(userRoles: string[], requiredRoles: string[]): boolean {
  if (requiredRoles.length === 0) return true;
  return requiredRoles.some(role => userRoles.includes(role));
}