import { Component, inject, OnInit, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { CommonModule } from '@angular/common';
import { ReservationService } from '../../core/services/reservation';
import { Reservation } from '../../shared/Models/Reservation';
import { Snackbar } from '../../core/services/snackbar';

@Component({
  selector: 'app-reservation',
  imports: [
    MatTableModule,
    MatCheckboxModule,
    MatPaginatorModule,
    CommonModule
],
  templateUrl: './reservations.html',
  styleUrl: './reservations.scss'
})
export class Reservations implements OnInit, AfterViewInit{
  @ViewChild(MatPaginator, { static: false }) paginator!: MatPaginator;
  
  reservationService = inject(ReservationService);
  snackbar = inject(Snackbar);
  cdr = inject(ChangeDetectorRef);
  dataSource = new MatTableDataSource<Reservation>([]);
  columnsToDisplay = ['ID', 'Email', 'Show', 'Volwassenen', 'Kinderen', 'Totaal', 'Code', 'isPaid'];

  private _reservations: Reservation[] = [];

  get reservations(): Reservation[] {
    return this._reservations;
  }

  set reservations(value: Reservation[]) {
    this._reservations = value;
    this.dataSource.data = value;
    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
  }

  ngOnInit(): void {
    this.reservationService.getReservations().subscribe({
      next: response => {
        this.reservations = response;
      },
      error: error => console.log('Error: ', error)
    })
  }

  ngAfterViewInit(): void {
    console.log('ngAfterViewInit called');
    console.log('Paginator available:', !!this.paginator);
    console.log('Data length:', this.dataSource.data.length);
    
    // Ensure paginator is connected
    this.dataSource.paginator = this.paginator;
    
    // If we already have data, trigger the setter again
    if (this._reservations.length > 0) {
      this.reservations = this._reservations;
    }
  }

  togglePaidStatus(reservation: Reservation): void {
    const originalStatus = reservation.isPaid;
    reservation.isPaid = !reservation.isPaid;

    this.reservationService.updateReservation(reservation).subscribe({
      next: (updatedReservation) => {
        const index = this.dataSource.data.findIndex((r: Reservation) => r.id === updatedReservation.id);
        if (index !== -1) {
          const updatedData = [...this.dataSource.data];
          updatedData[index] = updatedReservation;
          this.dataSource.data = updatedData;
        }
      },
      error: (error) => {
        reservation.isPaid = originalStatus;
        this.snackbar.error('Fout bij het bijwerken van betaalstatus');
      }
    });
  }
}

