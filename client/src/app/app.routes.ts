import { Routes } from '@angular/router';
import { Home } from './features/home/home';
import { Tickets } from './features/tickets/tickets';
import { Confirmation } from './features/confirmation/confirmation';

export const routes: Routes = [
    { path: '', component: Home },
    { path: 'tickets/:showid', component: Tickets},
    { path: 'confirmation', component: Confirmation}
];
