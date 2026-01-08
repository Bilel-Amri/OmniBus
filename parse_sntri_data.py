"""
Parse SNTRI CSV data and generate C# seed code for OmniBus
"""
import csv
from io import StringIO
from collections import defaultdict
from datetime import datetime, timedelta
import uuid

# Your CSV data (sample - add complete data)
sntri_csv = """Arret;Route;Ligne;Nom;Station;Aller;Retour;Lundi;Mardi;Mercredi;Jeudi;vendredi;samedi;Dimanche
1;GP1;100;TUNIS - GAFSA;TUNIS;06:00;16:40;*;*;*;*;*;*;*
17;GP3;100;TUNIS - GAFSA;GAFSA;11:40;11:00;*;*;*;*;*;*;*
1;GP1;102;TUNIS - NEFTA;TUNIS;13:00;18:00;*;*;*;*;*;*;*
23;GP3;102;TUNIS - NEFTA;NEFTA;20:30;10:30;*;*;*;*;*;*;*
1;GP1;103;SOUSSE - ZARZIS;SOUSSE;06:45;13:50;*;*;*;*;*;*;*
10;GP1;103;SOUSSE - ZARZIS;ZARZIS;13:25;07:10;*;*;*;*;*;*;*
1;GP1;104;TUNIS - B. GUARDEN;TUNIS;12:00;19:00;*;*;*;*;*;*;*
13;GP1;104;TUNIS - B. GUARDEN;B. GUARDEN;20:30;10:30;*;*;*;*;*;*;*
1;GP5;108;TUNIS - FOUSSANA;TUNIS;07:00;18:30;*;*;*;*;*;*;*
27;GP5;108;TUNIS - FOUSSANA;FOUSSANA;12:30;13:00;*;*;*;*;*;*;*
1;GP5;121;TUNIS - AIN DRAHAM;TUNIS;05:30;15:50;*;*;*;*;*;*;*
16;GP6;121;TUNIS - AIN DRAHAM;AIN DRAHAM;09:20;12:00;*;*;*;*;*;*;*
1;GP5;124;TUNIS - TABARKA;TUNIS;06:30;16:30;*;*;*;*;*;*;*
19;GP6;124;TUNIS - TABARKA;TABARKA;10:00;13:00;*;*;*;*;*;*;*
1;GP1;142;TUNIS -TOZEUR;TUNIS;08:00;15:00;*;*;*;*;*;*;*
21;GP3;142;TUNIS -TOZEUR;TOZEUR;15:00;08:00;*;*;*;*;*;*;*
1;GP1;189;TUNIS - TATAOUINE;TUNIS;09:30;16:15;*;*;*;*;*;*;*
20;GP1;189;TUNIS - TATAOUINE;TATAOUINE;18:15;07:30;*;*;*;*;*;*;*
1;GP1;194;TUNIS -ELFAOUAR;TUNIS;11:00;19:15;*;*;*;*;*;*;*
20;GP1;194;TUNIS -ELFAOUAR;ELFAOUAR;20:30;09:45;*;*;*;*;*;*;*"""

def parse_time(time_str):
    """Parse time string like '06:00' or '00:00'"""
    if not time_str or time_str.strip() == '':
        return None
    try:
        return datetime.strptime(time_str.strip(), '%H:%M').time()
    except:
        return None

def calculate_duration_minutes(start_time, end_time):
    """Calculate duration between two times"""
    if not start_time or not end_time:
        return 120  # Default 2 hours
    
    start = datetime.combine(datetime.today(), start_time)
    end = datetime.combine(datetime.today(), end_time)
    
    # Handle overnight trips
    if end < start:
        end += timedelta(days=1)
    
    duration = (end - start).total_seconds() / 60
    return int(duration)

def estimate_distance(duration_minutes):
    """Estimate distance based on average speed of 60-70 km/h"""
    return int(duration_minutes * 1.0)  # ~60-70 km/h average

def calculate_price(distance_km):
    """Calculate realistic price in TND based on distance"""
    base_price = 5.0
    price_per_km = 0.10
    return round(base_price + (distance_km * price_per_km), 2)

# Parse CSV
routes_data = defaultdict(list)
reader = csv.DictReader(StringIO(sntri_csv), delimiter=';')

for row in reader:
    ligne = row['Ligne']
    nom = row['Nom']
    station = row['Station']
    aller = row['Aller']
    retour = row['Retour']
    
    key = (ligne, nom)
    routes_data[key].append({
        'station': station,
        'aller': aller,
        'retour': retour
    })

# Generate C# code
print("// ====== SNTRI REAL ROUTES FROM CSV DATA ======")
print("var sntriRoutes = new List<Route>\n{")

route_configs = []

for idx, ((ligne, nom), stops) in enumerate(routes_data.items(), start=1):
    if len(stops) < 2:
        continue
    
    first_stop = stops[0]
    last_stop = stops[-1]
    
    origin = first_stop['station']
    destination = last_stop['station']
    
    # Parse times for outbound
    start_time = parse_time(first_stop['aller'])
    end_time = parse_time(last_stop['aller'])
    
    duration_out = calculate_duration_minutes(start_time, end_time)
    distance = estimate_distance(duration_out)
    price = calculate_price(distance)
    
    # Generate GUID
    route_guid = str(uuid.uuid4())
    
    origin_code = origin[:3].upper().replace(' ', '')
    dest_code = destination[:3].upper().replace(' ', '')
    
    print(f"    // Line {ligne}: {nom}")
    print(f"    new Route {{ ")
    print(f"        Id = Guid.Parse(\"{route_guid}\"),")
    print(f"        Name = \"{nom}\",")
    print(f"        Origin = \"{origin}\",")
    print(f"        OriginCode = \"{origin_code}\",")
    print(f"        Destination = \"{destination}\",")
    print(f"        DestinationCode = \"{dest_code}\",")
    print(f"        DistanceKm = {distance},")
    print(f"        EstimatedDurationMinutes = {duration_out},")
    print(f"        Description = \"SNTRI Line {ligne} - {len(stops)} stops\",")
    print(f"        IsActive = true,")
    print(f"        CreatedAt = seedDate,")
    print(f"        IsDeleted = false")
    print(f"    }},")
    
    # Store for schedule generation
    if start_time:
        route_configs.append({
            'ligne': ligne,
            'nom': nom,
            'route_id': route_guid,
            'departure_hour': start_time.hour,
            'price': price,
            'duration': duration_out
        })

print("};")
print("modelBuilder.Entity<Route>().HasData(sntriRoutes);")
print("\n\n// Schedule configurations for SNTRI routes")
for config in route_configs[:20]:  # First 20 as example
    print(f"// Line {config['ligne']}: {config['nom']} - departs at {config['departure_hour']:02d}:00, {config['price']} TND")

print("\n\n// Generate complete documentation")
print("// Total SNTRI routes parsed:", len(routes_data))
print("// Total route configurations:", len(route_configs))
