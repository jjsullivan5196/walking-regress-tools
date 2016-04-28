setwd("C:/Users/gameprogrammer/Documents/walking-regress-tools/Assets/Resources")
source("funcs.R")

smoothconstant = 0.003

byTime = TRUE

acc = read.csv("daniel-walking-acc.csv", header = FALSE, col.names = c("comx","comy","comz","time"))
gyro = read.csv("daniel-walking-gyro.csv", header = FALSE, col.names = c("comx","comy","comz","time"))

acc_smooth = data.frame(comx = predict(loess(comx ~ time, data = acc, span = smoothconstant)), comy = predict(loess(comy ~ time, data = acc, span = smoothconstant)), comz = predict(loess(comz ~ time, data = acc, span = smoothconstant)), time = acc$time)

#acc = zero_time(acc, 250, 284)
#gyro = zero_time(gyro, 250, 284)

par(mfcol = c(3, 2))

par(mfcol = c(3, 2))

if(!byTime) {
  plot(acc$comx, type = "l", col = "red")
  plot(acc$comy, type = "l", col = "green")
  plot(acc$comz, type = "l", col = "blue")
  
  plot(gyro$comx, type = "l", col = "red")
  plot(gyro$comy, type = "l", col = "green")
  plot(gyro$comz, type = "l", col = "blue")
}
if(byTime) {
  plot(acc$time, acc$comx, type = "l", col = "red")
  plot(acc$time, acc$comy, type = "l", col = "green")
  plot(acc$time, acc$comz, type = "l", col = "blue")
  
  plot(gyro$time, gyro$comx, type = "l", col = "red")
  plot(gyro$time, gyro$comy, type = "l", col = "green")
  plot(gyro$time, gyro$comz, type = "l", col = "blue")
}

write.table(acc ,"daniel-single-step2-acc.csv", sep = ',', col.names = FALSE, row.names = FALSE)
write.table(gyro ,"daniel-single-step2-gyro.csv", sep = ',', col.names = FALSE, row.names = FALSE)