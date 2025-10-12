import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Tickets } from './features/tickets/tickets';
import { Confirmation } from './features/confirmation/confirmation';
import { Login } from './features/account/login/login';
import { authGuard } from './core/guards/auth-guard';
import { Reservations } from './features/reservations/reservations';

export const routes: Routes = [
    { path: '', component: Home },
    { path: 'tickets/:showid', component: Tickets},
    { path: 'confirmation', component: Confirmation},
    { path: 'account/login', component: Login },
    { path: 'reservations', component: Reservations, canActivate: [authGuard] },
];
