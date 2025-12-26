import { CommonModule } from '@angular/common';
import { AfterViewInit, ChangeDetectorRef, Component, inject, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatButton } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon, MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { ReservationService } from '../../core/services/reservation';
import { Snackbar } from '../../core/services/snackbar';
import { Reservation } from '../../shared/Models/Reservation';
import { Show } from '../../shared/Models/Show';


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
    MatSlideToggleModule,
    MatButton,
    MatIcon
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
  columnsToDisplay = ['ID', 'Email', 'Surname', 'Name', 'Show', 'Volwassenen', 'Kinderen', 'Totaal', 'Code', 'isPaid', 'emailSent', 'Delete', 'Remark', 'Stoelen'];
  searchCode: string = ''
  filteredData: Reservation[] = [];
  showOnlyUnpaid: boolean = false;
  originalData: Reservation[] = [];
  sendingTickets: boolean = false;
  shows: Show[] = []

  sumTotalSaturday: number = 0;
  sumTotalSunday: number = 0;
  total: number = 0;

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

  calculateShowTotals(): void {
    const saturdayReservations = this.reservations.filter(r => r.showId === 1);
    const sundayReservations = this.reservations.filter(r => r.showId === 2);
    this.sumTotalSaturday = saturdayReservations.reduce((sum, r) => sum + (r.numberOfAdults ?? 0) + (r.numberOfChildren ?? 0), 0);
    this.sumTotalSunday = sundayReservations.reduce((sum, r) => sum + (r.numberOfAdults ?? 0) + (r.numberOfChildren ?? 0), 0);
    this.total = this.sumTotalSaturday + this.sumTotalSunday;
  }

  ngOnInit(): void {
    this.reservationService.emptyReservation();
    this.getReservations();
  }

  getReservations(): void {
    this.reservationService.getReservations().subscribe({
      next: response => {
        console.log('Reservations received:', response);
        this.reservations = response;
        this.calculateShowTotals();
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
        // Update the reservation in the main array and recalculate totals
        const mainIndex = this._reservations.findIndex(r => r.id === updatedReservation.id);
        if (mainIndex !== -1) {
          this._reservations[mainIndex] = updatedReservation;
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

  sendTickets(): void {
    this.sendingTickets = true;

    this.reservationService.sendTickets().subscribe({
      next: () => {
        this.snackbar.success('Tickets succesvol verzonden.');
        this.sendingTickets = false;
        this.getReservations();
      },
      error: (error) => {
        console.error('Error sending tickets:', error);
        this.snackbar.error('Fout bij het verzenden van tickets.');
        this.sendingTickets = false;
      }
    });
  }

  deleteReservation(id: number): void {
    this.reservationService.deleteReservation(id).subscribe({
      next: () => {
        this.snackbar.success('Reservering succesvol verwijderd.');
        this.getReservations();
      },
      error: (error) => {
        console.error('Error deleting reservation:', error);
        this.snackbar.error('Fout bij het verwijderen van de reservering.');
      }
    });
  }

  updateReservationRemark(reservation: Reservation, newRemark: string): void {
    const originalRemark = reservation.remark;
    reservation.remark = newRemark;

    this.reservationService.updateReservation(reservation).subscribe({
      next: (updatedReservation) => {
        const index = this.dataSource.data.findIndex((r: Reservation) => r.id === updatedReservation.id);
        if (index !== -1) {
          const updatedData = [...this.dataSource.data];
          updatedData[index] = updatedReservation;
          this.dataSource.data = updatedData;
        }
        // Update the reservation in the main array
        const mainIndex = this._reservations.findIndex(r => r.id === updatedReservation.id);
        if (mainIndex !== -1) {
          this._reservations[mainIndex] = updatedReservation;
        }
        this.snackbar.success('Opmerking bijgewerkt');
      },
      error: (error) => {
        reservation.remark = originalRemark;
        console.error('Error updating remark:', error);
        this.snackbar.error('Fout bij het bijwerken van opmerking');
      }
    });
  }
}