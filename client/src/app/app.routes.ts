import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Tickets } from './features/tickets/tickets';
import { Confirmation } from './features/confirmation/confirmation';
import { Login } from './features/account/login/login';
import { Reservations } from './features/reservations/reservations';
import { Seatoverview } from './features/seatoverview/seatoverview';
import { roleGuard } from './core/guards/role-guard';
import { Ticketinfo } from './features/ticketinfo/ticketinfo';

export const routes: Routes = [
    { path: '', component: Home },
    { path: 'tickets/:showid', component: Tickets},
    { path: 'confirmation', component: Confirmation},
    { path: 'account/login', component: Login },
    { 
        path: 'reservations', 
        component: Reservations, 
        canActivate: [roleGuard],
        data: { roles: ['Admin'] }
    },
    { path: 'seatoverview/:showid', component: Seatoverview, },
    { path: 'ticketinfo', component: Ticketinfo, },
]