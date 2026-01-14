#define _USE_MATH_DEFINES
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <math.h>

#ifndef M_PI
#define M_PI 3.14159265358979323846
#endif

#define VERSION "1.0.0"

void print_usage(const char *prog_name) {
    printf("Pipe Trades CLI - Field-calibrated pipefitter calculation ecosystem\n");
    printf("Version %s\n\n", VERSION);
    printf("Usage: %s <command> [options]\n\n", prog_name);
    printf("Commands:\n");
    printf("  gps-verify <lat1> <lon1> <lat2> <lon2>  - Verify GPS coordinates and calculate distance\n");
    printf("  beam-wrap <diameter> <length>             - Estimate beam wrap material needed\n");
    printf("  rolling-offset <offset> <roll> <travel>   - Calculate rolling offset dimensions\n");
    printf("  help                                      - Show this help message\n");
    printf("  version                                   - Show version information\n");
}

void print_version() {
    printf("Pipe Trades CLI version %s\n", VERSION);
    printf("Built for rope access crews doing fireproofing containment\n");
}

double gps_distance(double lat1, double lon1, double lat2, double lon2) {
    // Haversine formula for GPS distance calculation
    const double R = 6371000.0; // Earth radius in meters
    double lat1_rad = lat1 * M_PI / 180.0;
    double lat2_rad = lat2 * M_PI / 180.0;
    double delta_lat = (lat2 - lat1) * M_PI / 180.0;
    double delta_lon = (lon2 - lon1) * M_PI / 180.0;
    
    double a = sin(delta_lat / 2.0) * sin(delta_lat / 2.0) +
               cos(lat1_rad) * cos(lat2_rad) *
               sin(delta_lon / 2.0) * sin(delta_lon / 2.0);
    double c = 2.0 * atan2(sqrt(a), sqrt(1.0 - a));
    
    return R * c;
}

void cmd_gps_verify(int argc, char *argv[]) {
    if (argc < 6) {
        fprintf(stderr, "Error: gps-verify requires 4 coordinates (lat1 lon1 lat2 lon2)\n");
        exit(1);
    }
    
    double lat1 = atof(argv[2]);
    double lon1 = atof(argv[3]);
    double lat2 = atof(argv[4]);
    double lon2 = atof(argv[5]);
    
    double distance = gps_distance(lat1, lon1, lat2, lon2);
    
    printf("GPS Coordinate Verification\n");
    printf("============================\n");
    printf("Point 1: %.6f, %.6f\n", lat1, lon1);
    printf("Point 2: %.6f, %.6f\n", lat2, lon2);
    printf("Distance: %.2f meters (%.2f feet)\n", distance, distance * 3.28084);
}

void cmd_beam_wrap(int argc, char *argv[]) {
    if (argc < 4) {
        fprintf(stderr, "Error: beam-wrap requires diameter and length\n");
        exit(1);
    }
    
    double diameter = atof(argv[2]); // inches
    double length = atof(argv[3]);   // feet
    
    // Calculate surface area for wrap material
    double radius = diameter / 2.0;
    double circumference = 2.0 * M_PI * radius;
    double length_inches = length * 12.0;
    double area_sq_inches = circumference * length_inches;
    double area_sq_feet = area_sq_inches / 144.0;
    
    // Add 10% for overlap
    double material_needed = area_sq_feet * 1.1;
    
    printf("Beam Wrap Material Estimation\n");
    printf("==============================\n");
    printf("Beam diameter: %.2f inches\n", diameter);
    printf("Beam length: %.2f feet\n", length);
    printf("Circumference: %.2f inches\n", circumference);
    printf("Surface area: %.2f sq ft\n", area_sq_feet);
    printf("Material needed (with 10%% overlap): %.2f sq ft\n", material_needed);
}

void cmd_rolling_offset(int argc, char *argv[]) {
    if (argc < 5) {
        fprintf(stderr, "Error: rolling-offset requires offset, roll, and travel values\n");
        exit(1);
    }
    
    double offset = atof(argv[2]);  // inches
    double roll = atof(argv[3]);    // inches
    double travel = atof(argv[4]);  // inches
    
    // Calculate true offset using 3D Pythagorean theorem
    double true_offset = sqrt(offset * offset + roll * roll);
    
    // Calculate set (45-degree fitting advance)
    double set = (travel * travel - offset * offset - roll * roll) / (2.0 * travel);
    
    // Calculate diagonal travel
    double diagonal = sqrt(travel * travel + offset * offset + roll * roll - 2.0 * travel * set);
    
    printf("Rolling Offset Calculation\n");
    printf("===========================\n");
    printf("Offset: %.2f inches\n", offset);
    printf("Roll: %.2f inches\n", roll);
    printf("Travel: %.2f inches\n", travel);
    printf("True offset: %.2f inches\n", true_offset);
    printf("Set: %.2f inches\n", set);
    printf("Diagonal travel: %.2f inches\n", diagonal);
}

int main(int argc, char *argv[]) {
    if (argc < 2) {
        print_usage(argv[0]);
        return 1;
    }
    
    const char *command = argv[1];
    
    if (strcmp(command, "help") == 0 || strcmp(command, "--help") == 0 || strcmp(command, "-h") == 0) {
        print_usage(argv[0]);
        return 0;
    }
    
    if (strcmp(command, "version") == 0 || strcmp(command, "--version") == 0 || strcmp(command, "-v") == 0) {
        print_version();
        return 0;
    }
    
    if (strcmp(command, "gps-verify") == 0) {
        cmd_gps_verify(argc, argv);
        return 0;
    }
    
    if (strcmp(command, "beam-wrap") == 0) {
        cmd_beam_wrap(argc, argv);
        return 0;
    }
    
    if (strcmp(command, "rolling-offset") == 0) {
        cmd_rolling_offset(argc, argv);
        return 0;
    }
    
    fprintf(stderr, "Error: Unknown command '%s'\n\n", command);
    print_usage(argv[0]);
    return 1;
}
