import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SearchService } from '../../core/services/search.service';
import { Patient } from '../../models/patient.model';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search.html',
  styleUrl: './search.scss'
})
export class SearchComponent implements OnInit {

  private searchService = inject(SearchService);

  patients: Patient[] = [];
  searchText = '';

  ngOnInit(): void {
    console.log('ngOnInit called');
    this.loadPatients();
  }

  loadPatients(): void {
    console.log('Loading all patients');

    this.searchService.getAllPatients().subscribe({
      next: (data) => {
        console.log('All Patients:', data);
        this.patients = data;
      },
      error: (err) => console.error(err)
    });
  }

  search(): void {

    const searchValue = this.searchText.trim();

    if (!searchValue) {
      this.loadPatients();
      return;
    }

    this.searchService
      .searchPatients(searchValue)
      .subscribe({
        next: (data) => {
          this.patients = data;
        },
        error: (err) => {
          console.error(err);
        }
      });
  }
}