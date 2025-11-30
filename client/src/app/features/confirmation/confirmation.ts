import { Component, inject } from '@angular/core';
import { ReservationService } from '../../core/services/reservation';
import { CommonModule } from '@angular/common';
import { NavigationStart, Router } from '@angular/router';

@Component({
  selector: 'app-confirmation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirmation.html',
  styleUrls: ['./confirmation.scss']
})
export class Confirmation {
  reservationService = inject(ReservationService)

  constructor(private router: Router) {
    this.router.events.subscribe(event => {
      if (event instanceof NavigationStart && event.navigationTrigger === 'popstate') {
        this.router.navigateByUrl('/', { replaceUrl: true });
      }
    });
  }
}
