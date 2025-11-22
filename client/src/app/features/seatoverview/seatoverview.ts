import { CommonModule } from '@angular/common';
import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { ActivatedRoute, Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ReservationService } from '../../core/services/reservation';
import { SeatService } from '../../core/services/seat';
import { Snackbar } from '../../core/services/snackbar';
import { Seat } from '../../shared/Models/Seat';
import { SeatStatus } from '../../shared/Models/SeatStatus';

@Component({
  selector: 'app-seatoverview',
  imports: [CommonModule, FormsModule, MatButton],
  templateUrl: './seatoverview.html',
  styleUrl: './seatoverview.scss'
})
export class Seatoverview implements OnInit, OnDestroy {
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
  noBlockLeft: boolean = false;
  private isSubmitting: boolean = false;

  ngOnInit(): void {
    this.getShowInfo();
    this.loadSeatsForShow(this.showId);
  }

  ngOnDestroy(): void {
    // Only cleanup if not submitting (i.e., user is navigating away without reserving)
    if (!this.isSubmitting) {
      this.cleanupSelectedSeats();
    }
  }

  private cleanupSelectedSeats(): void {
    const seats = this.getSelectedSeats();
    
    if (seats.length === 0) {
      return;
    }

    const unholdRequests = seats.map(seat => {
      const body = {
        showId: this.showId,
        seatId: seat.id,
      };
      return this.seatService.unholdSeat(body);
    });

    forkJoin(unholdRequests).subscribe({
      next: () => {
        this.reservationService.emptyReservation();
      },
      error: (err) => {
        console.error('Error unholding seats:', err);
        // Still empty reservation even if unhold fails
        this.reservationService.emptyReservation();
      }
    });
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
        if (this.singleGapExists()) {
          this.noBlockLeft = true;
        }
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
      this.loading = false;
      return;
    }

    // Filter out wheelchair rows for autoselect
    const regularSeats = this.seats.filter(row =>
      row.length > 0 && !row[0].isWheelchair
    );

    for (const row of regularSeats) {
      for (let i = 0; i <= row.length - this.selectedSeatsCount; i++) {
        const block = row.slice(i, i + this.selectedSeatsCount)
        if (block.every(seat => seat.status === SeatStatus.Available && !seat.isWheelchair)) {
          if (this.wouldCreateSingleGap(row, i, this.selectedSeatsCount)) continue;
          this.holdRequests(block);
          return;
        }
      }
    }

    this.noBlockLeft = true
    const availableSeats: Seat[] = [];

    for (const row of regularSeats) {
      for (const seat of row) {
        if (seat.status === SeatStatus.Available && !seat.isWheelchair) {
          availableSeats.push(seat);
          if (availableSeats.length === this.selectedSeatsCount) {
            break;
          }
        }
      }
      if (availableSeats.length === this.selectedSeatsCount) {
        break;
      }
    }

    if (availableSeats.length > 0) {
      this.holdRequests(availableSeats)
    }
  }

  holdRequests(seats: Seat[]): void {
    const holdRequests = seats.map(seat => {
      const body = {
        showId: this.showId,
        seatId: seat.id,
      };
      return this.seatService.holdSeat(body);
    });

    forkJoin(holdRequests).subscribe({
      next: (responses) => {
        responses.forEach((response, idx) => {
          seats[idx].status = response;
        });
        this.loading = false;
      },
      error: (err) => {
        console.error('Error holding seats:', err);
        this.loading = false;
      }
    });
    return;
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
    if (!this.noBlockLeft && this.singleGapExists()) {
      this.snack.error('Reserveren is niet mogelijk als je een lege stoel overlaat.');
      return;
    }

    const reservationData = {
      reservation: this.reservationService.reservation(),
      seatIds: this.getSelectedSeats().map(seat => seat.id)
    }

    this.reservationService.createReservation(reservationData).subscribe({
      next: (data: any) => {
        this.isSubmitting = true;
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
          return true;
        }
      }
    }
    return false;
  }

  wouldCreateSingleGap(row: Seat[], startIndex: number, count: number): boolean {
    const endIndex = startIndex + count - 1;

    // Check left side: is there exactly one available seat before the block?
    const leftSeat = row[startIndex - 1];
    const leftLeftSeat = row[startIndex - 2];
    if (
      leftSeat?.status === SeatStatus.Available &&
      (!leftLeftSeat || leftLeftSeat.status !== SeatStatus.Available)
    ) {
      return true;
    }

    // Check right side: is there exactly one available seat after the block?
    const rightSeat = row[endIndex + 1];
    const rightRightSeat = row[endIndex + 2];
    if (
      rightSeat?.status === SeatStatus.Available &&
      (!rightRightSeat || rightRightSeat.status !== SeatStatus.Available)
    ) {
      return true;
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
      case SeatStatus.WheelchairAvailable:
        return 'Wheelchair Available';
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

  isWheelchairSeat(seat: Seat): boolean {
    return seat.isWheelchair;
  }

  cancel(): void {
    this.cleanupSelectedSeats();
    this.router.navigateByUrl('/');
  }
}
