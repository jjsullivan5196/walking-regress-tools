zero_time = function(tr_dat, idx1, idx2 = nrow(tr_dat)) {
  zero = tr_dat$time[idx1]
  tr_dat = tr_dat[idx1:idx2,]
  tr_dat$time = tr_dat$time - zero
  return(tr_dat)
}