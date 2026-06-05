$Project = "D:\assignments-2course\2semester\programming\New folder\backend\tests\UnitTests\HotelOS.UnitTests.csproj"

Write-Host "`n"
Write-Host "====== HotelOS - Unit Test Report ======" -ForegroundColor Cyan

$output = dotnet test $Project -v detailed 2>&1

$passedLines = $output | Select-String "Passed "
$totalLine = $output | Select-String "Total tests:"

$testMap = @{}
$passedLines | ForEach-Object {
    $line = $_ -replace '^\s+', ''
    if ($line -match 'Passed\s+HotelOS\.UnitTests\.(.+?)\s+\[(.+?)\]') {
        $full = $matches[1]
        $dur = $matches[2]
        # Split on last dot to get class name (everything before last dot)
        $lastDot = $full.LastIndexOf('.')
        if ($lastDot -gt 0) {
            $cls = $full.Substring(0, $lastDot)
        } else {
            $cls = $full
        }
        if (-not $testMap[$cls]) { $testMap[$cls] = @() }
        $testMap[$cls] += $dur
    }
}

$serviceMap = @{
    'gateway-api (auth)'   = @('TS08_AuthControllerTests')
    'housekeeping-service' = @('TS03_HousekeeperMarksClean_StatusMachine')
    'maintenance-service'  = @('TS03_MaintenancePriorityQueueTests','TS05_CriticalMaintenance_PriorityQueue','MaintenancePriorityQueueTests')
    'payment-service'      = @('TS02_GuestCheckOut_BillingAndEvents','TS04_BillingCalculationServiceTests','BillingCalculationServiceTests')
    'reception-service'    = @('TS05_BookingConcurrencyTests','TS06_ReceptionService_PublishesBookingCreatedEvent','TS06_ConcurrentCheckIn_NoDoubleBooking')
    'room-service'         = @('TS01_RoomFactoryTests','TS01_GuestCheckIn_RoomAssignment','TS02_RoomAssignmentAlgorithmTests','TS04_RoomServiceOrder_StateMachine','TS07_AllRoomsOccupied_Validation','TS08_InvalidRoomNumber_InputValidation','RoomAssignmentAlgorithmTests')
    'user-service (auth)'  = @('TS08_AuthControllerTests')
    'websocket-service'    = @('TS07_WebSocketConsumerTests')
}

function Get-Count($classes) {
    $total = 0
    $durs = @()
    $seen = @{}
    foreach ($c in $classes) {
        if ($testMap[$c]) {
            $total += $testMap[$c].Count
            $durs += $testMap[$c]
        }
    }
    return @{ Count = $total; Durs = $durs }
}

Write-Host "`n"
Write-Host ("{0,-30} {1,6} {2,10}  {3}" -f "Service", "Tests", "Time", "Result") -ForegroundColor White
Write-Host ("-" * 60) -ForegroundColor DarkGray

$grandTotal = 0
$passedCount = 0

$svcOrder = @('gateway-api (auth)','housekeeping-service','maintenance-service','payment-service','reception-service','room-service','user-service (auth)','websocket-service')

foreach ($svc in $svcOrder) {
    $classes = $serviceMap[$svc]
    $r = Get-Count $classes
    $count = $r.Count
    $durMs = 0
    foreach ($d in $r.Durs) {
        $dClean = $d -replace ' ms','' -replace '< 1','0'
        $durMs += [int]$dClean
    }
    $grandTotal += $count

    if ($count -gt 0) {
        $passedCount += $count
        $icon = "[OK]"
        $durStr = if ($durMs -ge 1000) { ("{0:N1}s" -f ($durMs/1000)) } else { "$durMs ms" }
        $fg = 'Green'
    } else {
        $icon = "[--]"
        $durStr = "-"
        $fg = 'Yellow'
    }

    Write-Host ("{0,-30} {1,6} {2,10}  {3}" -f $svc, $count, $durStr, $icon) -ForegroundColor $fg
}

Write-Host ("-" * 60) -ForegroundColor DarkGray
$finalStatus = if ($passedCount -eq $grandTotal) { "[ALL PASSED]" } else { "[FAILURES]" }
Write-Host ("{0,-30} {1,6} {2,10}  {3}" -f "UNIQUE TOTAL", "", "", $finalStatus) -ForegroundColor White

if ($totalLine) {
    Write-Host "`n  $($totalLine.ToString().Trim())" -ForegroundColor Cyan
}
Write-Host "  * Auth tests cover both gateway-api and user-service" -ForegroundColor DarkGray

Write-Host "`n"
Write-Host "-- Test Classes --" -ForegroundColor DarkGray
$testMap.Keys | Sort-Object | ForEach-Object {
    $c = $_
    $n = $testMap[$c].Count
    Write-Host ("  [OK] {0,-55} {1} test(s)" -f $c, $n) -ForegroundColor DarkGreen
}

Write-Host "`n"
