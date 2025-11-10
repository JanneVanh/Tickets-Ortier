import { Component, inject } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuItem, MatMenuTrigger } from '@angular/material/menu';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { Account } from '../../core/services/account';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    RouterLinkActive,
    MatIcon,
    MatMenuTrigger,
    MatButton,
    MatMenu,
    MatMenuItem
  ],
  templateUrl: './header.html',
  styleUrl: './header.scss'
})
export class Header {
  accountService = inject(Account)
  router = inject(Router)
  
  get canAccessReservations(): boolean {
    const user = this.accountService.currentUser();
    return user !== null && user.roles && user.roles.includes('Admin');
  }
  
  logout(){
    this.accountService.logout().subscribe({
      next: () => {
        this.accountService.currentUser.set(null);
        this.router.navigateByUrl('/');
      }
    })
  }
}
