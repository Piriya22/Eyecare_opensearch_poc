import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { Patient } from '../../models/patient.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SearchService {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/Search`;

  getAllPatients(): Observable<Patient[]> {
    return this.http.get<Patient[]>(`${this.apiUrl}/all`);
  }

  searchPatients(searchText: string): Observable<Patient[]> {
    return this.http.get<Patient[]>(
      `${this.apiUrl}?q=${encodeURIComponent(searchText)}`
    );
  }
}