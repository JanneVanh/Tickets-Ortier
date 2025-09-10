import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Tickets } from './features/tickets/tickets';

export const routes: Routes = [
    { path: '', component: Home },
    { path: 'tickets/:showid', component: Tickets}
];
