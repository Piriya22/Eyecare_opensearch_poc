export interface Patient {
    patientId: string;
    name: string;
    age: number;
    doctor: string;
    diagnosis: string;
    phone: string;
    appointmentDate: string;
    highlightName?: string;
    highlightDoctor?: string;
    highlightDiagnosis?: string;
}