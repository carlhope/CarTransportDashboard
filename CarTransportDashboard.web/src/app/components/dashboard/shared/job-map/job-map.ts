import { Component, OnInit } from '@angular/core';
import { localSecrets } from '../../../../localSecrets';

@Component({
  selector: 'app-job-map',
  standalone: true,
  templateUrl: './job-map.html',
  styleUrl: './job-map.scss'
})
export class JobMap implements OnInit {
  private scriptLoaded = false;

  async ngOnInit() {
    await this.loadGoogleMapsScript();
    this.initMap();
  }

  private loadGoogleMapsScript(): Promise<void> {
    console.log('Origin:', window.location.origin);
    console.log('key '+localSecrets.apiKey);

    if (this.scriptLoaded) return Promise.resolve();

    return new Promise((resolve, reject) => {
      const script = document.createElement('script');
      script.src = `https://maps.googleapis.com/maps/api/js?key=${localSecrets.apiKey}`;

      script.async = true;
      script.defer = true;

      script.onload = () => {
        this.scriptLoaded = true;
        resolve();
      };

      script.onerror = (err) => reject(err);

      document.head.appendChild(script);
    });
  }

  private initMap() {
    const mapElement = document.getElementById('map');

    if (!mapElement) return;

    const map = new google.maps.Map(mapElement,
      { center:
          {
            lat: 53.0027, lng: -2.1794
          },
            zoom: 10
      });

  }
}
