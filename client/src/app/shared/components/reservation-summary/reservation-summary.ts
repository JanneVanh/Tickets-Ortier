import { Component, inject } from '@angular/core';
import { ReservationService } from '../../../core/services/reservation';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-reservation-summary',
  standalone: true,
  imports: [
    CurrencyPipe
  ],
  templateUrl: './reservation-summary.html',
  styleUrl: './reservation-summary.scss'
})
export class ReservationSummary {
  reservationService = inject(ReservationService)
}
