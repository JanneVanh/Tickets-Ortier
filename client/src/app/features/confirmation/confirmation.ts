import { Component, inject } from '@angular/core';
import { ReservationService } from '../../core/services/reservation';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirmation',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './confirmation.html',
  styleUrls: ['./confirmation.scss']
})
export class Confirmation {
  reservationService = inject(ReservationService)
}
