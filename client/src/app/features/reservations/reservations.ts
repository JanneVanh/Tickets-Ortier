import { Component, inject, OnInit, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { MatTableModule, MatTableDataSource } from '@angular/material/table';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatPaginatorModule, MatPaginator } from '@angular/material/paginator';
import { CommonModule } from '@angular/common';
import { ReservationService } from '../../core/services/reservation';
import { Reservation } from '../../shared/Models/Reservation';
import { Snackbar } from '../../core/services/snackbar';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule } from '@angular/forms';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';


@Component({
  selector: 'app-reservation',
  imports: [
    MatTableModule,
    MatCheckboxModule,
    MatPaginatorModule,
    CommonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    FormsModule,
    MatSlideToggleModule
  ],
  templateUrl: './reservations.html',
  styleUrl: './reservations.scss'
})
export class Reservations implements OnInit, AfterViewInit {
  @ViewChild(MatPaginator, { static: false }) paginator!: MatPaginator;

  reservationService = inject(ReservationService);
  snackbar = inject(Snackbar);
  cdr = inject(ChangeDetectorRef);
  dataSource = new MatTableDataSource<Reservation>([]);
  columnsToDisplay = ['ID', 'Email', 'Show', 'Volwassenen', 'Kinderen', 'Totaal', 'Code', 'isPaid'];
  searchCode: string = ''
  filteredData: Reservation[] = [];
  showOnlyUnpaid: boolean = false;
  originalData: Reservation[] = [];

  private _reservations: Reservation[] = [];

  get reservations(): Reservation[] {
    return this._reservations;
  }

  set reservations(value: Reservation[]) {
    console.log('Setting reservations, received:', value?.length || 0, 'items');
    this._reservations = value;
    this.originalData = [...value]; // Store original data
    this.filteredData = [...value];

    this.applyFilters();
    if (this.paginator) {
      this.dataSource.paginator = this.paginator;
    }
  }

  ngOnInit(): void {
    console.log('ReservationComponent ngOnInit - fetching reservations...');
    this.reservationService.getReservations().subscribe({
      next: response => {
        console.log('Reservations received:', response);
        this.reservations = response;
      },
      error: error => {
        console.error('Error fetching reservations:', error);
        this.snackbar.error('Fout bij het ophalen van reserveringen');
      }
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

  filterByCode(): void {
    this.applyFilters();
  }

  toggleUnpaidFilter(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    console.log('Applying filters. Original data length:', this.originalData.length);
    console.log('Show only unpaid:', this.showOnlyUnpaid);
    console.log('Search code:', this.searchCode);

    let filtered = this.originalData;

    if (this.showOnlyUnpaid) {
      filtered = filtered.filter(reservation => !reservation.isPaid)
      console.log('After unpaid filter:', filtered.length);
    }

    this.filteredData = filtered;

    if (this.searchCode.trim()) {
      filtered = this.filteredData.filter(reservation =>
        reservation.paymentCode?.toLowerCase().includes(this.searchCode.toLowerCase())
      );
      console.log('After code filter:', filtered.length);
    }

    console.log('Final filtered data length:', filtered.length);
    this.dataSource.data = filtered;
  }
}

