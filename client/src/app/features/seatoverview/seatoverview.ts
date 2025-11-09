import { CommonModule } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { SeatService } from '../../core/services/seat';
import { Seat } from '../../shared/Models/Seat';
import { SeatStatus } from '../../shared/Models/SeatStatus';
import { MatButton } from '@angular/material/button';
import { ReservationService } from '../../core/services/reservation';
import { Snackbar } from '../../core/services/snackbar';

@Component({
  selector: 'app-seatoverview',
  imports: [CommonModule, FormsModule, MatButton],
  templateUrl: './seatoverview.html',
  styleUrl: './seatoverview.scss'
})
export class Seatoverview implements OnInit {
  private activatedRoute = inject(ActivatedRoute)
  seats: Seat[][] = [];
  showId: number = 0;
  showDay: string = "";
  loading = false;
  selectedSeatsCount: number = 0;

  private seatService = inject(SeatService);
  readonly SeatStatus = SeatStatus;
  reservationService = inject(ReservationService)
  private snack = inject(Snackbar)
  private router = inject(Router)

  ngOnInit(): void {
    this.getShowInfo();
    this.loadSeatsForShow(this.showId);
  }

  getShowInfo(): void {
    this.activatedRoute.paramMap.subscribe(params => {
      const showIdParam = params.get('showid');
      this.showId = showIdParam !== null ? Number(showIdParam) : 0;
    })

    switch (this.showId) {
      case 1: this.showDay = "zaterdag"
        break;
      case 2: this.showDay = "zondag"
        break;
    }
  }

  loadSeatsForShow(showId: number): void {
    this.loading = true;
    this.seats = [];

    this.seatService.getSeatsForShow(showId).subscribe({
      next: (data) => {
        const grouped = data.reduce((acc, seat) => {
          acc[seat.row] = acc[seat.row] || [];
          acc[seat.row].push(seat);
          return acc;
        }, {} as Record<string, Seat[]>);

        const sortedRows = Object.keys(grouped).sort();
        this.seats = sortedRows.map(key =>
          grouped[key].sort((a, b) => a.number - b.number)
        );
        this.loading = false;
        this.autoSelectSeats()
      },
      error: (error) => {
        console.error('Error loading seats:', error);
        this.loading = false;
      }
    });
  }

  autoSelectSeats() {
    this.selectedSeatsCount = this.reservationService.totalSeats() ?? 0;

    if (this.selectedSeatsCount <= 0 || this.seats.length === 0) {
      return;
    }

    for (const row of this.seats) {
      for (let i = 0; i <= row.length - this.selectedSeatsCount; i++) {
        const block = row.slice(i, i + this.selectedSeatsCount)
        if (block.every(seat => seat.status === SeatStatus.Available)) {
          block.forEach(seat => seat.status = SeatStatus.Selected)
          return
        }
      }
    }
  }

  selectSeat(seat: Seat): void {
    let onlyUnselect = false;
    if (seat.status === SeatStatus.Reserved || this.getSelectedSeatsCount() === this.reservationService.totalSeats()) {
      onlyUnselect = true;
    }

    const body = {
      showId: this.showId,
      seatId: seat.id,
    }

    if (seat.status === SeatStatus.Available && !onlyUnselect) {
      this.seatService.holdSeat(body).subscribe(response => {
        seat.status = response
      });
    } else if (seat.status === SeatStatus.Selected) {
      this.seatService.unholdSeat(body).subscribe(response => {
        seat.status = response
      })
    }
  }

  onSubmit(): void {
    if (this.singleGapExists()) return;

    const reservationData = {
      reservation: this.reservationService.reservation(),
      seatIds: this.getSelectedSeats().map(seat => seat.id)
    }

    this.reservationService.createReservation(reservationData).subscribe({
      next: (data: any) => {
        this.reservationService.reservation.set(data);
        this.snack.success('Je tickets zijn gereserveerd.');
        this.router.navigateByUrl('/confirmation');
      },
      error: (error) => {
        console.error('Reservation failed:', error);
        console.error('Error details:', error.error);
        this.snack.error('Er is een fout opgetreden bij het reserveren.');
      }
    });
  }

singleGapExists(): boolean {
  for (const row of this.seats) {
    for (let i = 0; i < row.length; i++) {
      const left = row[i - 1];
      const current = row[i];
      const right = row[i + 1];

      // Check if current is available but surrounded by selected seats
      if (
        current?.status === SeatStatus.Available &&
        (!left || left?.status === SeatStatus.Selected || left?.status === SeatStatus.Reserved) &&
        (!right || right?.status === SeatStatus.Selected || right?.status === SeatStatus.Reserved)
      ) {
        this.snack.error('Reservering niet mogelijk: één lege stoel tussen geselecteerde stoelen.');
        return true;
      }
    }
  }
  return false;
}

  getRowLabel(index: number): string {
    return String.fromCharCode(65 + index);
  }

  getSeatStatusText(status: SeatStatus): string {
    switch (status) {
      case SeatStatus.Available:
        return 'Available';
      case SeatStatus.Reserved:
        return 'Reserved';
      case SeatStatus.Selected:
        return 'Selected';
      default:
        return 'Unknown';
    }
  }

  trackBySeat(index: number, seat: Seat): string {
    return `${seat.row}-${seat.number}`;
  }

  trackByRow(index: number, row: Seat[]): string {
    return `${index}-${row}`;
  }

  getSelectedSeats(): Seat[] {
    return this.seats.flat().filter(seat => seat.status === SeatStatus.Selected);
  }

  getSelectedSeatsCount(): number {
    return this.getSelectedSeats().length;
  }
}
