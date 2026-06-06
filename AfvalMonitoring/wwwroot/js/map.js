// Google Maps initialization for trash locations
let map;
let googleMapsApiKey = null;
let googleMapsReady = false;
let pendingMarkers = null;

// Set the Google Maps API key (called from Blazor component)
window.setGoogleMapsKey = function(key) {
    console.log('API key set in JavaScript');
    googleMapsApiKey = key;

    // Dynamically load Google Maps script with the correct API key
    if (!window.google) {
        const script = document.createElement('script');
        script.src = `https://maps.googleapis.com/maps/api/js?key=${key}`;
        script.async = true;
        script.defer = false;
        script.onload = function() {
            console.log('Google Maps API script loaded');
            googleMapsReady = true;
            // If markers are pending, render them now
            if (pendingMarkers) {
                console.log('Rendering pending markers');
                window.initTrashMap(pendingMarkers);
            }
        };
        script.onerror = function() {
            console.error('Failed to load Google Maps API script');
        };
        document.head.appendChild(script);
    } else {
        googleMapsReady = true;
    }
};

window.initTrashMap = function(markers) {
    console.log('initTrashMap called with', markers ? markers.length : 0, 'markers');
    if (markers && markers.length > 0) {
        console.log('First marker:', JSON.stringify(markers[0]));
    }

    // Check if map element exists
    const mapElement = document.getElementById('map');
    if (!mapElement) {
        console.error('Map element not found');
        return;
    }
    console.log('Map element found');

    // Check if Google Maps is loaded - if not, queue the markers
    if (!googleMapsReady || typeof google === 'undefined' || typeof google.maps === 'undefined') {
        console.warn('Google Maps API not ready yet, queueing markers');
        pendingMarkers = markers;
        return;
    }
    console.log('Google Maps API loaded');

    if (!markers || markers.length === 0) {
        console.warn('No markers provided');
        console.log('markers object:', markers);
        mapElement.innerHTML = '<div style="padding: 20px; color: orange;"><strong>Waarschuwing:</strong> Geen markeerpunten beschikbaar. Controleer of de backend draait op http://127.0.0.1:8000/ en of er data in de database staat.</div>';
        return;
    }
    console.log('Markers provided:', markers.length);

    // Default center (Breda, Netherlands)
    const defaultCenter = { lat: 51.5761, lng: 4.7817 };

    // Test backend connectivity
    testBackendConnection();

    try {
        console.log('Creating map');
        // Create map
        map = new google.maps.Map(mapElement, {
            zoom: 12,
            center: defaultCenter,
            styles: [
                {
                    "featureType": "water",
                    "elementType": "geometry",
                    "stylers": [{ "color": "#e9e9e9" }, { "lightness": 17 }]
                },
                {
                    "featureType": "landscape",
                    "elementType": "geometry",
                    "stylers": [{ "color": "#f3f3f3" }, { "lightness": 20 }]
                },
                {
                    "featureType": "road.highway",
                    "elementType": "geometry.fill",
                    "stylers": [{ "color": "#ffffff" }, { "lightness": 17 }]
                },
                {
                    "featureType": "road.arterial",
                    "elementType": "geometry.fill",
                    "stylers": [{ "color": "#ffffff" }, { "lightness": 16 }]
                },
                {
                    "featureType": "road.local",
                    "elementType": "geometry.fill",
                    "stylers": [{ "color": "#ffffff" }, { "lightness": 16 }]
                }
            ]
        });

        console.log('Map created successfully');

        // Add markers for each trash location
        const bounds = new google.maps.LatLngBounds();
        let validMarkers = 0;

        markers.forEach((marker, index) => {
            if (!marker.lat || !marker.lng) {
                console.warn(`Marker ${index} without coordinates:`, marker);
                return;
            }

            validMarkers++;
            const position = { lat: parseFloat(marker.lat), lng: parseFloat(marker.lng) };
            const confidenceColor = getConfidenceColor(marker.confidence);

            console.log(`Adding marker ${index}: ${marker.label} at (${position.lat}, ${position.lng})`);

            // Create custom marker with trash bin icon
            const trashMarker = new google.maps.Marker({
                position: position,
                map: map,
                title: marker.label,
                icon: {
                    url: 'data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiIHN0YW5kYWxvbmU9Im5vIj8+PHN2ZyB3aWR0aD0iODAwcHgiIGhlaWdodD0iODAwcHgiIHZpZXdCb3g9IjAgMCAyNCAyNCIgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZlcnNpb249IjEuMSIgeG1sbnM6Y2M9Imh0dHA6Ly9jcmVhdGl2ZWNvbW1vbnMub3JnL25zIyIgeG1sbnM6ZGM9Imh0dHA6Ly9wdXJsLm9yZy9kYy9lbGVtZW50cy8xLjEvIj48ZyB0cmFuc2Zvcm09InRyYW5zbGF0ZSgwIC0xMDI4LjQpIj48cGF0aCBkPSJtMTIgMGMtNC40MTgzIDIuMzY4NWUtMTUgLTggMy41ODE3LTggOCAwIDEuNDIxIDAuMzgxNiAyLjc1IDEuMDMxMiAzLjkwNiAwLjEwNzkgMC4xOTIgMC4yMjEgMC4zODEgMC4zNDM4IDAuNTYzbDYuNjI1IDExLjUzMSA2LjYyNS0xMS41MzFjMC4xMDItMC4xNTEgMC4xOS0wLjMxMSAwLjI4MS0wLjQ2OWwwLjA2My0wLjA5NGMwLjY0OS0xLjE1NiAxLjAzMS0yLjQ4NSAxLjAzMS0zLjkwNiAwLTQuNDE4My0zLjU4Mi04LTgtOHptMCA0YzIuMjA5IDAgNCAxLjc5MDkgNCA0IDAgMi4yMDktMS43OTEgNC00IDQtMi4yMDkxIDAtNC0xLjc5MS00LTQgMC0yLjIwOTEgMS43OTA5LTQgNC00eiIgdHJhbnNmb3JtPSJ0cmFuc2xhdGUoMCAxMDI4LjQpIiBmaWxsPSIjZTc0YzNjIi8+PHBhdGggZD0ibTEyIDNjLTIuNzYxNCAwLTUgMi4yMzg2LTUgNSAwIDIuNzYxIDIuMjM4NiA1IDUgNSAyLjc2MSAwIDUtMi4yMzkgNS01IDAtMi43NjE0LTIuMjM5LTUtNS01em0wIDJjMS42NTcgMCAzIDEuMzQzMSAzIDNzLTEuMzQzIDMtMyAzLTMtMS4zNDMxLTMtMyAxLjM0My0zIDMtM3oiIHRyYW5zZm9ybT0idHJhbnNsYXRlKDAgMTAyOC40KSIgZmlsbD0iI2MwMzkyYiIvPjwvZz48L3N2Zz4=',
                    scaledSize: new google.maps.Size(32, 32),
                    anchor: new google.maps.Point(16, 32)
                }
            });

            // Add info window
            const infoContent = `
                <div style="padding: 10px; max-width: 250px;">
                    <h6 style="margin: 0 0 8px 0; font-weight: bold;">${marker.label || 'Unknown'}</h6>
                    <p style="margin: 4px 0; font-size: 12px;">
                        <strong>Betrouwbaarheid:</strong> ${marker.confidence ? (marker.confidence * 100).toFixed(0) + '%' : 'N/A'}
                    </p>
                    ${marker.adres ? `<p style="margin: 4px 0; font-size: 12px;"><strong>Adres:</strong> ${marker.adres}</p>` : ''}
                    <p style="margin: 4px 0; font-size: 11px; color: #666;">
                        ${position.lat.toFixed(6)}, ${position.lng.toFixed(6)}
                    </p>
                </div>
            `;

            const infoWindow = new google.maps.InfoWindow({
                content: infoContent
            });

            trashMarker.addListener('click', () => {
                infoWindow.open(map, trashMarker);
            });

            bounds.extend(position);
        });

        console.log(`Added ${validMarkers} valid markers to map`);

        // Fit map to all markers
        if (validMarkers > 0) {
            map.fitBounds(bounds);

            // Add padding to the bounds
            const listener = map.addListener('idle', () => {
                google.maps.event.removeListener(listener);
                const zoomLevel = map.getZoom();
                if (zoomLevel > 15) {
                    map.setZoom(15);
                }
            });
        } else {
            console.warn('No valid markers with coordinates found');
        }
    } catch (error) {
        console.error('Error initializing map:', error);
        mapElement.innerHTML = '<div style="padding: 20px; color: red;"><strong>Fout bij kaartinitialisatie:</strong> ' + error.message + '</div>';
    }
}

// Test if backend is reachable
async function testBackendConnection() {
    try {
        const response = await fetch('http://127.0.0.1:8000/');
        if (response.ok) {
            console.log('Backend is reachable at http://127.0.0.1:8000/');
        } else {
            console.error('Backend returned status:', response.status);
        }
    } catch (error) {
        console.error('Cannot reach backend at http://127.0.0.1:8000/', error.message);
        console.error('Make sure the FastAPI backend is running');
    }
}
function getConfidenceColor(confidence) {
    if (!confidence) return '#cccccc'; // Gray - no confidence
    const conf = parseFloat(confidence);
    if (conf >= 0.8) return '#28a745'; // Green - high confidence
    if (conf >= 0.6) return '#17a2b8'; // Teal - medium-high
    if (conf >= 0.4) return '#ffc107'; // Yellow - medium
    return '#dc3545'; // Red - low confidence
}

