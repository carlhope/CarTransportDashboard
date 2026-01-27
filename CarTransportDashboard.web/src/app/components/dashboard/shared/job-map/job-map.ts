import {Component, ElementRef, Input, OnChanges, OnInit, SimpleChanges, ViewChild} from '@angular/core';
import { localSecrets } from '../../../../localSecrets';

@Component({
  selector: 'app-job-map',
  standalone: true,
  templateUrl: './job-map.html',
  styleUrl: './job-map.scss'
})
export class JobMap implements OnInit, OnChanges {
  @Input() polyline: string | null | undefined = null;
  private map: google.maps.Map | null = null;
  @ViewChild('mapContainer', { static: true }) mapElement!: ElementRef<HTMLDivElement>;
  private scriptLoaded = false;
  private currentPolyline: google.maps.Polyline | null = null;
  private readonly fallbackCenter = { lat: 53.0027, lng: -2.1794 };


  async ngOnInit() {
    await this.loadGoogleMapsScript();
    this.initMap();
  }
  ngOnChanges(changes: SimpleChanges) {
    if (changes['polyline'] && this.map) {
      this.renderPolyline();
    }
  }
  private loadGoogleMapsScript(): Promise<void> {
    if (this.scriptLoaded) return Promise.resolve();
    return new Promise((resolve, reject) => {
      const script = document.createElement('script');
      script.src = `https://maps.googleapis.com/maps/api/js?key=${localSecrets.googleMapsApiKey}&libraries=geometry`;
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
    const mapElement = this.mapElement.nativeElement;


    if (!mapElement) return;

    this.map = new google.maps.Map(mapElement, {
      center: this.fallbackCenter,
      zoom: 10
    });

    this.renderPolyline();
  }

  private renderPolyline() {
    if (!this.map) return;

    // Remove previous polyline if it exists
    if (this.currentPolyline) {
      this.currentPolyline.setMap(null);
      this.currentPolyline = null;
    }


    // Fallback if no polyline provided
    if (!this.polyline) {
      this.map.setCenter(this.fallbackCenter);
      this.map.setZoom(10);
      return;
    }

    // Decode encoded polyline string
    let decoded;
    try {
      decoded = google.maps.geometry.encoding.decodePath(this.polyline);
    } catch {
      this.map.setCenter(this.fallbackCenter);
      this.map.setZoom(10);
      return;
    }


    // Convert to plain objects
    const path = decoded.map(p => ({
      lat: p.lat(),
      lng: p.lng()
    }));

    const polyline = new google.maps.Polyline({
      path,
      geodesic: true,
      strokeColor: '#4285F4',
      strokeOpacity: 1.0,
      strokeWeight: 4
    });

    polyline.setMap(this.map);
    // Save reference so we can remove it later
    this.currentPolyline = polyline;


    // Fit map to route
    const bounds = new google.maps.LatLngBounds();
    path.forEach(point => bounds.extend(point));
    this.map.fitBounds(bounds);
  }

}
