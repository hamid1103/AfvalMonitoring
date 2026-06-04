// Google Maps initialization for trash locations
let map;

window.initTrashMap = function(markers) {
    console.log('=== MAP.JS DEBUG ===');
    console.log('initTrashMap called with', markers ? markers.length : 0, 'markers');
    if (markers && markers.length > 0) {
        console.log('First marker:', JSON.stringify(markers[0]));
    }

    // Check if map element exists
    const mapElement = document.getElementById('map');
    if (!mapElement) {
        console.error('❌ Map element not found');
        return;
    }
    console.log('✓ Map element found');

    // Check if Google Maps is loaded
    if (typeof google === 'undefined' || typeof google.maps === 'undefined') {
        console.error('❌ Google Maps API not loaded');
        mapElement.innerHTML = '<div style="padding: 20px; color: red;"><strong>Fout:</strong> Google Maps API niet geladen. Check je api key en internet verbinding.</div>';
        return;
    }
    console.log('✓ Google Maps API loaded');

    if (!markers || markers.length === 0) {
        console.warn('⚠ No markers provided');
        console.log('markers object:', markers);
        mapElement.innerHTML = '<div style="padding: 20px; color: orange;"><strong>Waarschuwing:</strong> Geen markeerpunten beschikbaar. Controleer of de backend draait op http://127.0.0.1:8000/ en of er data in de database staat.</div>';
        return;
    }
    console.log('✓ Markers provided:', markers.length);

    // Default center (Breda, Netherlands)
    const defaultCenter = { lat: 51.5761, lng: 4.7817 };

    // Test backend connectivity
    testBackendConnection();

    try {
        console.log('Creating map...');
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
                    url: 'data:image/svg+xml;base64,PHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHdpZHRoPSIyNCIgaGVpZ2h0PSIyNCIgdmlld0JveD0iMCAwIDI0IDI0Ij48cmVjdCB3aWR0aD0iMjQiIGhlaWdodD0iMjQiIGZpbGw9Im5vbmUiLz48cGF0aCBkPSJNMyA2aDE4TTggNnYtMmEyIDIgMCAwIDEgMi0yaDRhMiAyIDAgMCAxIDIgMnYyTTUgNnYxMGEyIDIgMCAwIDAgMiAyaDEwYTIgMiAwIDAgMCAyLTJWNk03IDZoMTBNMTAgNnY4TTEwIDEwaDRNMTAuNSA2djEwTTEzLjUgNnYxMCIgc3Ryb2tlPSIjRjU0MzM2IiBzdHJva2Utd2lkdGg9IjIiIHN0cm9rZS1saW5lY2FwPSJyb3VuZCIgc3Ryb2tlLWxpbmVqb2luPSJyb3VuZCIgZmlsbD0ibm9uZSIvPjwvc3ZnPg==',
                    scaledSize: new google.maps.Size(24, 24),
                    anchor: new google.maps.Point(12, 24)
                }
            });

            // Add info window
            const infoContent = `
                <div style="padding: 10px; max-width: 250px;">
                    <h6 style="margin: 0 0 8px 0; font-weight: bold;">${marker.label || 'Unknown'}</h6>
                    <p style="margin: 4px 0; font-size: 12px;">
                        <strong>Betrouwbaarheid:</strong> ${marker.confidence ? (marker.confidence * 100).toFixed(0) + '%' : 'N/A'}
                    </p>
                    ${marker.locatie ? `<p style="margin: 4px 0; font-size: 12px;"><strong>Adres:</strong> ${marker.locatie}</p>` : ''}
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
            console.log('✓ Backend is reachable at http://127.0.0.1:8000/');
        } else {
            console.error('❌ Backend returned status:', response.status);
        }
    } catch (error) {
        console.error('❌ Cannot reach backend at http://127.0.0.1:8000/', error.message);
        console.error('Make sure the FastAPI backend is running!');
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

