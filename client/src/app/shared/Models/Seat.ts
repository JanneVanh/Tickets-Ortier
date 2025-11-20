import { SeatStatus } from "./SeatStatus"

export type Seat = {
    id: number,
    row: string
    number: number
    isWheelchair: boolean
    status: SeatStatus
}