setwd("C:/xampp/htdocs/upload/")
source("funcs.R")

byTime = FALSE

acc = read.csv("acc1460650099.csv", header = FALSE, col.names = c("comx","comy","comz","time"))
#gyro = read.csv("john-walking-gyro.csv", header = FALSE, col.names = c("comx","comy","comz","time"))

#acc = zero_time(acc, 100)
#gyro = zero_time(gyro, 100)

par(mfrow = c(3, 1))

if(!byTime) {
  plot(acc$comx, type = "l", col = "red")
  plot(acc$comy, type = "l", col = "green")
  plot(acc$comz, type = "l", col = "blue")
}
if(byTime) {
  plot(acc$time, acc$comx, type = "l", col = "red")
  plot(acc$time, acc$comy, type = "l", col = "green")
  plot(acc$time, acc$comz, type = "l", col = "blue")
}
