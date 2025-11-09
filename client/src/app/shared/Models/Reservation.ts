export type Reservation = {
    id: number;
    email: string;
    showId: number;
    numberOfAdults: number;
    numberOfChildren: number;
    paymentCode: string | null;
    isPaid: boolean;
    surName: string;
    name: string;
    remark: string;
    reservationDate: string;
    totalPrice: number;
}