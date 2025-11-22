import { Component, inject, OnInit } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatMenu, MatMenuItem, MatMenuTrigger } from '@angular/material/menu';
import { NavigationEnd, Router, RouterLink, RouterLinkActive } from '@angular/router';
import { Account } from '../../core/services/account';
import { Title } from '@angular/platform-browser';
import { filter } from 'rxjs';

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
export class Header implements OnInit {
  accountService = inject(Account)
  router = inject(Router)
  titleService = inject(Title)
  mobileMenuOpen = false;
  pageTitle = '';
  
  ngOnInit() {
    this.updatePageTitle();
    this.router.events.pipe(
      filter(event => event instanceof NavigationEnd)
    ).subscribe(() => {
      this.updatePageTitle();
    });
  }
  
  updatePageTitle() {
    const route = this.router.url;
    if (route === '/' || route.startsWith('/?')) {
      this.pageTitle = 'Home';
    } else if (route.startsWith('/wiezijnwij')) {
      this.pageTitle = 'Wie zijn wij?';
    } else if (route.startsWith('/ticketinfo')) {
      this.pageTitle = 'Tickets';
    } else if (route.startsWith('/reservations')) {
      this.pageTitle = 'Reservations';
    } else if (route.startsWith('/tickets')) {
      this.pageTitle = 'Tickets';
    } else {
      this.pageTitle = '';
    }
  }
  
  get canAccessReservations(): boolean {
    const user = this.accountService.currentUser();
    return user !== null && user.roles && user.roles.includes('Admin');
  }
  
  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }
  
  closeMobileMenu() {
    this.mobileMenuOpen = false;
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
