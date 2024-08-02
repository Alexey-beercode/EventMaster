
export interface EventResponseDTO {
  id: string;
  name: string;
  description: string;
  date: Date;
  location: Location;
  maxParticipants: number;
  imageBase64: string;
  categoryId: string;
}
